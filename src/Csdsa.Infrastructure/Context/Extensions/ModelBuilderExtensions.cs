using Microsoft.EntityFrameworkCore;

namespace Csdsa.Domain.Context.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // Add role/user/enum seed data here as needed
        }

        public static void ConfigureDecimalPrecision(this ModelBuilder modelBuilder)
        {
            foreach (
                var property in modelBuilder
                    .Model.GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))
            )
            {
                property.SetColumnType("numeric(18,2)");
            }
        }

        public static void ConfigureStringLengths(this ModelBuilder modelBuilder)
        {
            foreach (
                var property in modelBuilder
                    .Model.GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(string) && p.GetMaxLength() == null)
            )
            {
                property.SetMaxLength(500);
            }
        }

        public static void ConfigureUtcDateTime(this ModelBuilder modelBuilder)
        {
            foreach (
                var property in modelBuilder
                    .Model.GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
            )
            {
                property.SetColumnType("timestamp with time zone");
            }
        }
    }
}
