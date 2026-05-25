using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Pb56_AttachmentUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PB-56: dropujemo staru nvarchar kolonu (do sada se nije popunjavala validnim UserId-jem)
            // i dodajemo nullable int UserId sa FK na Users tabelu.
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Attachments");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_UserId",
                table: "Attachments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Users_UserId",
                table: "Attachments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Users_UserId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_UserId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Attachments");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
