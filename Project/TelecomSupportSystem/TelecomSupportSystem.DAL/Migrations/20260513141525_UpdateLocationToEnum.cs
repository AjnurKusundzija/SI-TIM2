using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelecomSupportSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLocationToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Konvertuj postojeće string vrijednosti u odgovarajuće int vrijednosti enum-a
            migrationBuilder.Sql(@"
                UPDATE [Users] SET [Location] =
                    CASE [Location]
                        WHEN 'SARAJEVO'   THEN '0'
                        WHEN 'BANJA_LUKA' THEN '1'
                        WHEN 'TUZLA'      THEN '2'
                        WHEN 'ZENICA'     THEN '3'
                        WHEN 'MOSTAR'     THEN '4'
                        WHEN 'BIJELJINA'  THEN '5'
                        WHEN 'BRCKO'      THEN '6'
                        WHEN 'BIHAC'      THEN '7'
                        WHEN 'PRIJEDOR'   THEN '8'
                        WHEN 'DOBOJ'      THEN '9'
                        ELSE '0'
                    END
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Location",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 500);
        }
    }
}
