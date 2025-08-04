using Csdsa.Application.Interfaces;
using Csdsa.Infrastructure.Persistence;
using Csdsa.Infrastructure.Persistence.Context;
using Csdsa.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Csdsa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDatabase(configuration);
        services.AddPasswordHashing(configuration);
        services.AddRepositories();
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = BuildConnectionString(configuration);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                builder =>
                {
                    builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    builder.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null
                    );
                }
            )
        );

        return services;
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "CsdsaDb";
        var username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres";
        var password =
            Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException("DB_PASSWORD environment variable is required");
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

        return $"Host={host};Database={database};Username={username};Password={password};Port={port}";
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddPasswordHashing(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var workFactor = configuration.GetValue<int?>("Security:PasswordHashWorkFactor") ?? 12;

        services.AddScoped<IPasswordHasher>(provider =>
        {
            var logger = provider.GetService<ILogger<PasswordHasher>>();
            return new PasswordHasher(logger, workFactor);
        });

        return services;
    }
}
