using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogur.Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenseStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Licenses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Licenses");
        }
    }
}
