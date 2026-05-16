using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketIdToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "Notifications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Notifications");
        }
    }
}
