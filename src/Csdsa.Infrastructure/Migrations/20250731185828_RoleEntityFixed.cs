using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Csdsa.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RoleEntityFixed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                id = table.Column<Guid>(
                    type: "uuid",
                    nullable: false,
                    defaultValueSql: "gen_random_uuid()"
                ),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_roles = table.Column<int>(type: "integer", nullable: false),
                role_name = table.Column<string>(
                    type: "character varying(500)",
                    maxLength: 500,
                    nullable: false
                ),
                created_at = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false,
                    defaultValueSql: "TIMEZONE('utc', now())"
                ),
                updated_at = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: true
                ),
                is_deleted = table.Column<bool>(
                    type: "boolean",
                    nullable: false,
                    defaultValue: false
                ),
                created_by = table.Column<string>(
                    type: "character varying(450)",
                    maxLength: 450,
                    nullable: true
                ),
                updated_by = table.Column<string>(
                    type: "character varying(450)",
                    maxLength: 450,
                    nullable: true
                ),
            },
            constraints: table =>
            {
                table.PrimaryKey("p_k_roles", x => x.id);
            }
        );

        migrationBuilder.CreateIndex(
            name: "ix_roles_created_at",
            table: "roles",
            column: "created_at"
        );

        migrationBuilder.CreateIndex(
            name: "ix_roles_is_deleted",
            table: "roles",
            column: "is_deleted"
        );

        migrationBuilder.CreateIndex(
            name: "ix_roles_is_deleted_created_at",
            table: "roles",
            columns: new[] { "is_deleted", "created_at" }
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "roles");
    }
}
