using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Csdsa.Infrastructure.Context;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var connectionString = BuildConnectionString();

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string BuildConnectionString()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "CsdsaDb";
        var username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "mava";
        var password =
            Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException(
                "DB_PASSWORD environment variable is required for migrations"
            );
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

        return $"Host={host};Database={database};Username={username};Password={password};Port={port}";
    }
}
