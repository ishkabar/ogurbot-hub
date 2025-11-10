using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ogur.Hub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVpsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VpsContainers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContainerId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Image = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CpuUsagePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MemoryUsageBytes = table.Column<long>(type: "bigint", nullable: false),
                    MemoryLimitBytes = table.Column<long>(type: "bigint", nullable: false),
                    NetworkRxBytes = table.Column<long>(type: "bigint", nullable: false),
                    NetworkTxBytes = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpsContainers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VpsResourceSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CpuUsagePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MemoryTotalBytes = table.Column<long>(type: "bigint", nullable: false),
                    MemoryUsedBytes = table.Column<long>(type: "bigint", nullable: false),
                    DiskTotalBytes = table.Column<long>(type: "bigint", nullable: false),
                    DiskUsedBytes = table.Column<long>(type: "bigint", nullable: false),
                    NetworkRxBytesPerSec = table.Column<long>(type: "bigint", nullable: false),
                    NetworkTxBytesPerSec = table.Column<long>(type: "bigint", nullable: false),
                    LoadAverage1Min = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LoadAverage5Min = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LoadAverage15Min = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpsResourceSnapshots", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VpsWebsites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Domain = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServiceName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContainerId = table.Column<int>(type: "int", nullable: true),
                    SslEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SslExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LastCheckedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastStatusCode = table.Column<int>(type: "int", nullable: true),
                    LastResponseTimeMs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpsWebsites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VpsWebsites_VpsContainers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "VpsContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_VpsContainers_ContainerId",
                table: "VpsContainers",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_VpsContainers_Name",
                table: "VpsContainers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_VpsResourceSnapshots_Timestamp",
                table: "VpsResourceSnapshots",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_VpsWebsites_ContainerId",
                table: "VpsWebsites",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_VpsWebsites_Domain",
                table: "VpsWebsites",
                column: "Domain",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VpsResourceSnapshots");

            migrationBuilder.DropTable(
                name: "VpsWebsites");

            migrationBuilder.DropTable(
                name: "VpsContainers");
        }
    }
}
