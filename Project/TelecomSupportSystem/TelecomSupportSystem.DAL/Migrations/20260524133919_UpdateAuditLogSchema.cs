using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditLogSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Table",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "LogId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Table",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "AuditLogs",
                newName: "OldValue");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "AuditLogs",
                newName: "NewValue");

            migrationBuilder.RenameColumn(
                name: "IpAdress",
                table: "AuditLogs",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "ActionDate",
                table: "AuditLogs",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "AuditLogId",
                table: "AuditLogs",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_ActionDate",
                table: "AuditLogs",
                newName: "IX_AuditLogs_Timestamp");

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "AuditLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AuditLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "AuditLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionType",
                table: "AuditLogs",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ActionType",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "AuditLogs",
                newName: "ActionDate");

            migrationBuilder.RenameColumn(
                name: "OldValue",
                table: "AuditLogs",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "NewValue",
                table: "AuditLogs",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "AuditLogs",
                newName: "IpAdress");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AuditLogs",
                newName: "AuditLogId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                newName: "IX_AuditLogs_ActionDate");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LogId",
                table: "AuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Table",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Table",
                table: "AuditLogs",
                column: "Table");
        }
    }
}
