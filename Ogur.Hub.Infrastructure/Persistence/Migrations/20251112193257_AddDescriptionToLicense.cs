using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogur.Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToLicense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Licenses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Licenses");
        }
    }
}
