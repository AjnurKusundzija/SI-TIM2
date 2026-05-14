using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Teams_TeamId",
                table: "TicketUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Tickets_TicketId",
                table: "TicketUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketUser",
                table: "TicketUser");

            migrationBuilder.RenameTable(
                name: "TicketUser",
                newName: "TicketUsers");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUser_UserId",
                table: "TicketUsers",
                newName: "IX_TicketUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUser_TicketId",
                table: "TicketUsers",
                newName: "IX_TicketUsers_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUser_TeamId",
                table: "TicketUsers",
                newName: "IX_TicketUsers_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketUsers",
                table: "TicketUsers",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUsers_Teams_TeamId",
                table: "TicketUsers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUsers_Tickets_TicketId",
                table: "TicketUsers",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUsers_Users_UserId",
                table: "TicketUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketUsers_Teams_TeamId",
                table: "TicketUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUsers_Tickets_TicketId",
                table: "TicketUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUsers_Users_UserId",
                table: "TicketUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketUsers",
                table: "TicketUsers");

            migrationBuilder.RenameTable(
                name: "TicketUsers",
                newName: "TicketUser");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUsers_UserId",
                table: "TicketUser",
                newName: "IX_TicketUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUsers_TicketId",
                table: "TicketUser",
                newName: "IX_TicketUser_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUsers_TeamId",
                table: "TicketUser",
                newName: "IX_TicketUser_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketUser",
                table: "TicketUser",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Teams_TeamId",
                table: "TicketUser",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Tickets_TicketId",
                table: "TicketUser",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
