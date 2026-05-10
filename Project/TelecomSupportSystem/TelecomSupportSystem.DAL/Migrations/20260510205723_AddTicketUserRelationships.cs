using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Teams_TeamId",
                table: "TicketUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Teams_TeamId",
                table: "TicketUser",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Teams_TeamId",
                table: "TicketUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Teams_TeamId",
                table: "TicketUser",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
