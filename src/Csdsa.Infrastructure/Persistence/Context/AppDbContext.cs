using System.Security.Claims;
using Csdsa.Domain.Models;
using Csdsa.Domain.Models.Auth;
using Csdsa.Infrastructure.Persistence.Context.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Csdsa.Infrastructure.Persistence.Context
{
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

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<BlacklistedToken> BlacklistedTokens => Set<BlacklistedToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("pgcrypto");

            ConfigurePostgreSQL(modelBuilder);
            ConfigureBaseEntities(modelBuilder);
            ConfigureAuthEntities(modelBuilder);
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

            // Permission configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Resource).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
            });

            // UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity
                    .HasOne(e => e.User)
                    .WithMany(u => u.UserRoles) // Corrected from e.Role to u.UserRoles
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RolePermission configuration
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });
                entity
                    .HasOne(e => e.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RefreshToken configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.Token).IsUnique();
                entity.Property(e => e.Token).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CreatedByIp).HasMaxLength(45);
                entity.Property(e => e.RevokedByIp).HasMaxLength(45);
                entity
                    .HasOne(e => e.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // BlacklistedToken configuration
            modelBuilder.Entity<BlacklistedToken>(entity =>
            {
                entity.HasIndex(e => e.TokenId).IsUnique();
                entity.Property(e => e.TokenId).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(255);
                entity
                    .HasOne(e => e.User)
                    .WithMany(u => u.BlacklistedTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // ... rest of your existing methods remain the same ...

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
                }
            }
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var tableName = ToSnakeCase(
                        entityType.GetTableName() ?? entityType.DisplayName()
                    );

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
                            System.Reflection.BindingFlags.NonPublic
                                | System.Reflection.BindingFlags.Static
                        )
                        ?.MakeGenericMethod(entityType.ClrType);

                    var filter = method?.Invoke(null, Array.Empty<object>());
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter((dynamic?)filter);
                }
            }
        }

        private static System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : BaseEntity
        {
            System.Linq.Expressions.Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
            return filter;
        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i > 0)
                    result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            return result.ToString();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
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
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = userId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = userId;
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
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
    }
}
