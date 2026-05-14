using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketClosureWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClosedById",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClosureRequestStatus",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClosureRequestedById",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosureRequestedDate",
                table: "Tickets",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedById",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ClosureRequestStatus",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ClosureRequestedById",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ClosureRequestedDate",
                table: "Tickets");
        }
    }
}
