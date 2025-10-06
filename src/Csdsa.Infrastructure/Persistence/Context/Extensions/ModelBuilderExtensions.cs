using Csdsa.Domain.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Csdsa.Infrastructure.Persistence.Context.Extensions;

public static class ModelBuilderExtensions
{
    public static void ConfigureDecimalPrecision(this ModelBuilder modelBuilder)
    {
        // Add your existing decimal precision configuration
    }

    public static void ConfigureStringLengths(this ModelBuilder modelBuilder)
    {
        // Add your existing string length configuration
    }

    public static void ConfigureUtcDateTime(this ModelBuilder modelBuilder)
    {
        // Add your existing UTC datetime configuration
    }

    public static void SeedData(this ModelBuilder modelBuilder)
    {
        // Seed default roles
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        modelBuilder
            .Entity<Role>()
            .HasData(
                new Role
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    Description = "Administrator role with full access",
                    CreatedAt = DateTime.UtcNow,
                },
                new Role
                {
                    Id = userRoleId,
                    Name = "User",
                    Description = "Standard user role",
                    CreatedAt = DateTime.UtcNow,
                }
            );

        // Seed permissions
        var permissions = new[]
        {
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Users.Read",
                Resource = "Users",
                Action = "Read",
                Description = "Read user data",
                CreatedAt = DateTime.UtcNow,
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Users.Write",
                Resource = "Users",
                Action = "Write",
                Description = "Create and update users",
                CreatedAt = DateTime.UtcNow,
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Users.Delete",
                Resource = "Users",
                Action = "Delete",
                Description = "Delete users",
                CreatedAt = DateTime.UtcNow,
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Roles.Read",
                Resource = "Roles",
                Action = "Read",
                Description = "Read roles",
                CreatedAt = DateTime.UtcNow,
            },
            new Permission
            {
                Id = Guid.NewGuid(),
                Name = "Roles.Write",
                Resource = "Roles",
                Action = "Write",
                Description = "Create and update roles",
                CreatedAt = DateTime.UtcNow,
            },
        };

        modelBuilder.Entity<Permission>().HasData(permissions);

        // Assign all permissions to Admin role
        var rolePermissions = permissions
            .Select(p => new RolePermission
            {
                RoleId = adminRoleId,
                PermissionId = p.Id,
                AssignedAt = DateTime.UtcNow,
            })
            .ToArray();

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
    }
}
