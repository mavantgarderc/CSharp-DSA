using System.IO;
using System.Security.Cryptography;
using System.Threading.RateLimiting;
using Csdsa.Application.Behaviors;
using Csdsa.Application.Interfaces;
using Csdsa.Infrastructure.Auth.Services;
using Csdsa.Infrastructure.Persistence;
using Csdsa.Infrastructure.Persistence.Context;
using Csdsa.Infrastructure.Persistence.Repositories;
using Csdsa.Infrastructure.Repositories.EntityRepositories;
using DotNetEnv;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace Csdsa.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("=== STARTING APPLICATION ===");

            // Load environment variables from .env file
            Console.WriteLine("Loading environment variables...");
            Env.Load();
            Console.WriteLine("Environment variables loaded");

            // Build connection string from environment variables
            var connectionString =
                $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Database={Environment.GetEnvironmentVariable("DB_DATABASE")};Username={Environment.GetEnvironmentVariable("DB_USERNAME")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};Port={Environment.GetEnvironmentVariable("DB_PORT")}";
            Console.WriteLine(
                $"Connection string built: Host={Environment.GetEnvironmentVariable("DB_HOST")};Database={Environment.GetEnvironmentVariable("DB_DATABASE")};Port={Environment.GetEnvironmentVariable("DB_PORT")}"
            );
            var builder = WebApplication.CreateBuilder(args);
            var env = builder.Environment;
            Console.WriteLine($"Environment: {env.EnvironmentName}");

            // --- Serilog ---
            Console.WriteLine("Configuring Serilog...");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Csdsa.Api")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
                )
                .WriteTo.PostgreSQL(
                    connectionString: connectionString,
                    tableName: "logs",
                    needAutoCreateTable: true
                )
                .CreateLogger();
            Console.WriteLine("Serilog configured");

            builder.Host.UseSerilog();
            Console.WriteLine("Serilog host configured");

            // --- Controllers ---
            Console.WriteLine("Configuring controllers...");
            builder
                .Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = false;
                });
            Console.WriteLine("Controllers configured");

            // --- DbContext ---
            Console.WriteLine("Configuring DbContext...");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );
            Console.WriteLine("DbContext configured");

            // --- HttpContextAccessor ---
            Console.WriteLine("Configuring HttpContextAccessor...");
            builder.Services.AddHttpContextAccessor();
            Console.WriteLine("HttpContextAccessor configured");

            // --- Data Protection ---
            Console.WriteLine("Configuring Data Protection...");
            var dataProtectionKeysPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".csdsa-keys",
                "data-protection"
            );
            Directory.CreateDirectory(dataProtectionKeysPath);
            builder
                .Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
                .SetApplicationName("Csdsa.Api");
            Console.WriteLine("Data Protection configured");

            // --- MediatR with Behaviors ---
            Console.WriteLine("Configuring MediatR...");
            builder.Services.AddMediatR(typeof(Program).Assembly);
            Console.WriteLine("MediatR configured");

            // --- Dependency Injection ---
            Console.WriteLine("Configuring Dependency Injection...");
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            Console.WriteLine("Dependency Injection configured");

            // --- JWT Authentication (RSA Public Key) ---
            Console.WriteLine("Configuring JWT Authentication...");
            var jwtSettings = builder.Configuration.GetSection("JWT");

            // Try to get from environment variable first, then from config
            var publicKeyString =
                Environment.GetEnvironmentVariable("CSDSA_JWT_PUBLIC_KEY")
                ?? jwtSettings["PublicKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            Console.WriteLine($"JWT Issuer: {issuer}");
            Console.WriteLine($"JWT Audience: {audience}");
            Console.WriteLine($"JWT PublicKey length: {publicKeyString?.Length ?? 0}");

            if (string.IsNullOrEmpty(publicKeyString))
            {
                throw new InvalidOperationException(
                    "JWT PublicKey not found. Set CSDSA_JWT_PUBLIC_KEY environment variable or add JWT:PublicKey to appsettings.json"
                );
            }

            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException(
                    "JWT:Issuer configuration is missing. Please add it to appsettings.json"
                );
            }

            if (string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException(
                    "JWT:Audience configuration is missing. Please add it to appsettings.json"
                );
            }

            Console.WriteLine("Creating RSA key...");
            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKeyString), out _);
            Console.WriteLine("RSA key created");

            builder
                .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            builder.Services.AddAuthorization();
            Console.WriteLine("JWT Authentication configured");

            // --- Rate Limiting ---
            Console.WriteLine("Configuring Rate Limiting...");
            var isDev = env.IsDevelopment();
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy(
                    "AuthPolicy",
                    context =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: context.Connection.RemoteIpAddress?.ToString()
                                ?? "anonymous",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = isDev ? 20 : 5,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 5,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            }
                        )
                );
            });
            Console.WriteLine("Rate Limiting configured");

            // --- Swagger ---
            Console.WriteLine("Configuring Swagger...");
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Csdsa API", Version = "v1" });

                opt.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description = "Enter 'Bearer' followed by your JWT access token.",
                    }
                );

                opt.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    }
                );
            });
            Console.WriteLine("Swagger configured");

            // --- App Pipeline ---
            Console.WriteLine("Building application...");
            var app = builder.Build();
            Console.WriteLine("Application built successfully");

            Console.WriteLine("Configuring middleware pipeline...");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                Console.WriteLine("Swagger middleware configured");
            }
            else
            {
                app.UseExceptionHandler("/error");
                Console.WriteLine("Exception handler configured");
            }

            app.UseSerilogRequestLogging();
            Console.WriteLine("Serilog request logging configured");

            app.UseHttpsRedirection();
            Console.WriteLine("HTTPS redirection configured");

            app.UseRateLimiter();
            Console.WriteLine("Rate limiter configured");

            app.UseAuthentication();
            Console.WriteLine("Authentication configured");

            app.UseAuthorization();
            Console.WriteLine("Authorization configured");

            app.MapControllers();
            Console.WriteLine("Controllers mapped");

            Console.WriteLine("=== WEB APPLICATION Initiated ===");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== APPLICATION STARTUP FAILED ===");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }
}
