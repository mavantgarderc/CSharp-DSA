using System.Security.Cryptography;
using System.Threading.RateLimiting;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.Auth.Login;
using Csdsa.Infrastructure.Auth.Services;
using Csdsa.Infrastructure.Persistence;
using Csdsa.Infrastructure.Persistence.Context;
using Csdsa.Infrastructure.Persistence.Repositories;
using Csdsa.Infrastructure.Repositories.EntityRepositories;
using Csdsa.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        var builder = WebApplication.CreateBuilder(args);
        var env = builder.Environment;

        // --- Serilog ---
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Csdsa.Api")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            )
            .WriteTo.PostgreSQL(
                connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
                tableName: "logs",
                needAutoCreateTable: true
            )
            .CreateLogger();

        builder.Host.UseSerilog();

        // --- Controllers ---
        builder
            .Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });

        // --- DbContext ---
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        // --- HttpContextAccessor ---
        builder.Services.AddHttpContextAccessor();

        // --- MediatR ---
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
        });

        // --- Dependency Injection ---
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IEmailService, EmailService>();

        // --- JWT Authentication (RSA Public Key) ---
        var jwtSettings = builder.Configuration.GetSection("JWT");
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(Convert.FromBase64String(jwtSettings["PublicKey"]!), out _);

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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ClockSkew = TimeSpan.Zero,
                };
            });

        builder.Services.AddAuthorization();

        // --- Rate Limiting ---
        var isDev = env.IsDevelopment();
        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy(
                "AuthPolicy",
                context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
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

        // --- Swagger ---
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

        // --- App Pipeline ---
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }
}
