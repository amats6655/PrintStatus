using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrintStatus.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasePrinters_Locations_LocationId",
                table: "BasePrinters");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BasePrinters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "BasePrinters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_BasePrinters_Locations_LocationId",
                table: "BasePrinters",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasePrinters_Locations_LocationId",
                table: "BasePrinters");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "BasePrinters",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "BasePrinters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BasePrinters_Locations_LocationId",
                table: "BasePrinters",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
