using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "SubscriptionPackages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "SubscriptionPackages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "SubscriptionPackages");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "SubscriptionPackages");
        }
    }
}
