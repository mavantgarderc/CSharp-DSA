using System.Security.Claims;
using Csdsa.Domain.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Csdsa.Domain.Context
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

        // Add DbSets here as you create entities

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PostgreSQL-Specific settings
            ConfigurePostgreSQL(modelBuilder);

            // Configure base entity properties for all entities
            ConfigureBaseEntities(modelBuilder);

            // Configure indexes for better performance
            ConfigureIndexes(modelBuilder);

            // Configure relationships & constraints
            ConfigureRelationships(modelBuilder);

            // Add query filters for soft delete
            ConfigureQueryFilters(modelBuilder);
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
                        .HasDefaultValueSql("gen_random_uuid()");

                    modelBuilder
                        .Entity(entityType.ClrType)
                        .Property("CreatedAt")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
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
                    modelBuilder
                        .Entity(entityType.ClrType)
                        .HasIndex("CreatedAt")
                        .HasDatabaseName(
                            $"ix_{ToSnakeCase(entityType.GetTableName() ?? entityType.DisplayName())}_created_at"
                        );

                    modelBuilder
                        .Entity(entityType.ClrType)
                        .HasIndex("IsDeleted")
                        .HasDatabaseName(
                            $"ix_{ToSnakeCase(entityType.GetTableName() ?? entityType.DisplayName())}_is_deleted"
                        );

                    modelBuilder
                        .Entity(entityType.ClrType)
                        .HasIndex("IsDeleted", "CreatedAt")
                        .HasDatabaseName(
                            $"ix_{ToSnakeCase(entityType.GetTableName() ?? entityType.DisplayName())}_is_deleted_created_at"
                        );
                }
            }
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
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
            System.Linq.Expressions.Expression<System.Func<TEntity, bool>> filter = x =>
                !x.IsDeleted;
            return filter;
        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var result = new System.Text.StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i > 0)
                {
                    result.Append('_');
                }
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
            where T : BaseEntity
        {
            return Set<T>().IgnoreQueryFilters();
        }

        public IQueryable<T> OnlyDeleted<T>()
            where T : BaseEntity
        {
            return Set<T>().IgnoreQueryFilters().Where(x => x.IsDeleted);
        }
    }
}
