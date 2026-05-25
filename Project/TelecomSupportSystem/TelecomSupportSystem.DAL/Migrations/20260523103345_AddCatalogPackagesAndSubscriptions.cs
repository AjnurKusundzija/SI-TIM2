using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogPackagesAndSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogPackages",
                columns: table => new
                {
                    CatalogPackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogPackages", x => x.CatalogPackageId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAuditLogs",
                columns: table => new
                {
                    SubscriptionAuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    CatalogPackageId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAuditLogs", x => x.SubscriptionAuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "ClientSubscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogPackageId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeactivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSubscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_ClientSubscriptions_CatalogPackages_CatalogPackageId",
                        column: x => x.CatalogPackageId,
                        principalTable: "CatalogPackages",
                        principalColumn: "CatalogPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPackages_Status",
                table: "CatalogPackages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPackages_Type",
                table: "CatalogPackages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriptions_CatalogPackageId",
                table: "ClientSubscriptions",
                column: "CatalogPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriptions_Status",
                table: "ClientSubscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSubscriptions_UserId",
                table: "ClientSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAuditLogs_AdminId",
                table: "SubscriptionAuditLogs",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAuditLogs_CatalogPackageId",
                table: "SubscriptionAuditLogs",
                column: "CatalogPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAuditLogs_Timestamp",
                table: "SubscriptionAuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAuditLogs_UserId",
                table: "SubscriptionAuditLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientSubscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionAuditLogs");

            migrationBuilder.DropTable(
                name: "CatalogPackages");
        }
    }
}
