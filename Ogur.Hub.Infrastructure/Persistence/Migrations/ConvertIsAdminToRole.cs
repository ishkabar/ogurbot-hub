// File: Ogur.Hub.Infrastructure/Persistence/Migrations/YYYYMMDDHHMMSS_ConvertIsAdminToRole.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Migrations

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogur.Hub.Infrastructure.Persistence.Migrations;

/// <summary>
/// Migration to convert IsAdmin boolean to Role enum.
/// </summary>
public partial class ConvertIsAdminToRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add new Role column as int
        migrationBuilder.AddColumn<int>(
            name: "Role",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 1); // UserRole.User = 1

        // Convert existing IsAdmin values to Role
        // IsAdmin = true  → Role = 3 (Admin)
        // IsAdmin = false → Role = 1 (User)
        migrationBuilder.Sql(@"
            UPDATE Users 
            SET Role = CASE 
                WHEN IsAdmin = 1 THEN 3
                ELSE 1
            END
        ");

        // Drop old IsAdmin column
        migrationBuilder.DropColumn(
            name: "IsAdmin",
            table: "Users");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Add back IsAdmin column
        migrationBuilder.AddColumn<bool>(
            name: "IsAdmin",
            table: "Users",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: false);

        // Convert Role back to IsAdmin
        // Role = 3 (Admin) → IsAdmin = true
        // All others       → IsAdmin = false
        migrationBuilder.Sql(@"
            UPDATE Users 
            SET IsAdmin = CASE 
                WHEN Role = 3 THEN 1
                ELSE 0
            END
        ");

        // Drop Role column
        migrationBuilder.DropColumn(
            name: "Role",
            table: "Users");
    }
}
