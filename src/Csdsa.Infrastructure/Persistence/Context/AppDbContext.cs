using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Csdsa.Domain.Common;
using Csdsa.Domain.Entities;
using Csdsa.Domain.Exceptions;
using Csdsa.Domain.Aggregates.GraphAggregate;
using Csdsa.Domain.Aggregates.TreeAggregate;
using Csdsa.Infrastructure.Persistence.Context.Extensions;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;


namespace Csdsa.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IHttpContextAccessor httpContextAccessor
    )
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Auth DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    // public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BlacklistedToken> BlacklistedTokens => Set<BlacklistedToken>();

    // DSA DbSets (example; adjust for generics/owned types)
    // public DbSet<Graph> Graphs => Set<Graph>();
    // public DbSet<BinaryTree> BinaryTrees => Set<BinaryTree>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pgcrypto");

        ConfigurePostgreSQL(modelBuilder);
        ConfigureBaseEntities(modelBuilder);
        ConfigureAuthEntities(modelBuilder);
        ConfigureDsaEntities(modelBuilder);
        ConfigureIndexes(modelBuilder);
        ConfigureRelationships(modelBuilder);
        ConfigureQueryFilters(modelBuilder);

        modelBuilder.ConfigureDecimalPrecision();
        modelBuilder.ConfigureStringLengths();
        modelBuilder.ConfigureUtcDateTime();
        modelBuilder.SeedData();
    }

    private void ConfigureAuthEntities(ModelBuilder modelBuilder)
    {
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        // // Permission configuration
        // modelBuilder.Entity<Permission>(entity =>
        // {
        //     entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
        //     entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        //     entity.Property(e => e.Resource).HasMaxLength(50).IsRequired();
        //     entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
        // });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity
                .HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity
                .HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // // RolePermission configuration
        // modelBuilder.Entity<RolePermission>(entity =>
        // {
        //     entity.HasKey(e => new { e.RoleId, e.PermissionId });
        //     entity
        //         .HasOne(e => e.Role)
        //         .WithMany(r => r.RolePermissions)
        //         .HasForeignKey(e => e.RoleId)
        //         .OnDelete(DeleteBehavior.Cascade);
        //     entity
        //         .HasOne(e => e.Permission)
        //         .WithMany(p => p.RolePermissions)
        //         .HasForeignKey(e => e.PermissionId)
        //         .OnDelete(DeleteBehavior.Cascade);
        // });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.Token).HasMaxLength(255).IsRequired();
            // entity.Property(e => e.CreatedByIp).HasMaxLength(45);
            // entity.Property(e => e.RevokedByIp).HasMaxLength(45);
            entity
                .HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BlacklistedToken configuration
        modelBuilder.Entity<BlacklistedToken>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.Token).HasMaxLength(255).IsRequired();
            // entity.Property(e => e.Reason).HasMaxLength(255);
            entity
                .HasOne(e => e.User)
                .WithMany(u => u.BlacklistedTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureDsaEntities(ModelBuilder modelBuilder)
    {
        // Graph configuration (example; assumes non-generic for EF)
    //     modelBuilder.Entity<Graph>(entity =>
    //     {
    //         entity.HasKey(e => e.Id);
    //         entity.OwnsMany(e => e.Nodes, n =>
    //         {
    //             n.WithOwner().HasForeignKey("GraphId");
    //             n.HasKey("Id");
    //         });
    //         entity.OwnsMany(e => e.Edges, e =>
    //         {
    //             e.WithOwner().HasForeignKey("GraphId");
    //             e.HasKey("Id");
    //         });
    //     });
    //
    //     // BinaryTree configuration
    //     modelBuilder.Entity<BinaryTree>(entity =>
    //     {
    //         entity.HasKey(e => e.Id);
    //         entity.OwnsOne(e => e.Root, r =>
    //         {
    //             r.WithOwner().HasForeignKey("BinaryTreeId");
    //             r.HasKey("Id");
    //         });
    //     });
    }

    private void ConfigurePostgreSQL(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName() ?? entity.DisplayName()));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName() ?? $"pk_{entity.GetTableName()}"));
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(
                    ToSnakeCase(
                        foreignKey.GetConstraintName()
                            ?? $"fk_{entity.GetTableName()}_{foreignKey.PrincipalEntityType.GetTableName()}"
                    )
                );
            }

            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(
                    ToSnakeCase(
                        index.GetDatabaseName()
                            ?? $"ix_{entity.GetTableName()}_{string.Join("_", index.Properties.Select(p => p.Name))}"
                    )
                );
            }
        }
    }

    private void ConfigureBaseEntities(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasKey("Id");

                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("Id")
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("gen_random_uuid()");

                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("CreatedAt")
                    .HasDefaultValueSql("TIMEZONE('utc', now())")
                    .ValueGeneratedOnAdd();

                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("UpdatedAt")
                    .ValueGeneratedOnUpdate();

                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("IsDeleted")
                    .HasDefaultValue(false);

                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("CreatedBy")
                    .HasMaxLength(450)
                    .IsRequired(false);

                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("UpdatedBy")
                    .HasMaxLength(450)
                    .IsRequired(false);

                // Concurrency token
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property<byte[]>("RowVersion")
                    .IsRowVersion()
                    .IsConcurrencyToken();
            }
        }
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var tableName = ToSnakeCase(entityType.GetTableName() ?? entityType.DisplayName());

                modelBuilder
                    .Entity(entityType.ClrType)
                    .HasIndex("CreatedAt")
                    .HasDatabaseName($"ix_{tableName}_created_at");

                modelBuilder
                    .Entity(entityType.ClrType)
                    .HasIndex("IsDeleted")
                    .HasDatabaseName($"ix_{tableName}_is_deleted");

                modelBuilder
                    .Entity(entityType.ClrType)
                    .HasIndex("IsDeleted", "CreatedAt")
                    .HasDatabaseName($"ix_{tableName}_is_deleted_created_at");
            }
        }
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // add fluent relationships here when needed
    }

    private void ConfigureQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(
                        nameof(GetSoftDeleteFilter),
                        BindingFlags.NonPublic
                            | BindingFlags.Static
                    )
                    ?.MakeGenericMethod(entityType.ClrType);

                var filter = method?.Invoke(null, Array.Empty<object>());
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter((LambdaExpression)filter!);
            }
        }
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : BaseEntity
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
                result.Append('_');
            result.Append(char.ToLower(input[i]));
        }
        return result.ToString();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var userId = GetCurrentUserId();
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // entry.Entity.CreatedAt = DateTime.UtcNow;
                    // entry.Entity.CreatedBy = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // entry.Entity.UpdatedBy = userId;
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    // entry.Property(x => x.CreatedBy).IsModified = false;
                    break;
            }
        }
    }

    private string? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            return httpContext.User.FindFirst("UserId")?.Value
                ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        return null;
    }

    public IQueryable<T> IncludeDeleted<T>()
        where T : BaseEntity => Set<T>().IgnoreQueryFilters();

    public IQueryable<T> OnlyDeleted<T>()
        where T : BaseEntity => Set<T>().IgnoreQueryFilters().Where(x => x.IsDeleted);

    // New: Batch update for productivity
    public async Task<int> BatchUpdateAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateExpression)
        where T : class
    {
        return await Set<T>().Where(predicate).ExecuteUpdateAsync(updateExpression);
    }

    // New: No-tracking query for read-only operations
    public IQueryable<T> AsNoTracking<T>() where T : class => Set<T>().AsNoTracking();
}
