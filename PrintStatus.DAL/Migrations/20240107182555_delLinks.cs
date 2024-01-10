using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrintStatus.DAL.Migrations
{
    /// <inheritdoc />
    public partial class delLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_BasePrinters_BasePrinterId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Histories_Oids_OidId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_OidId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_BasePrinterId",
                table: "AuditLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Histories_OidId",
                table: "Histories",
                column: "OidId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_BasePrinterId",
                table: "AuditLogs",
                column: "BasePrinterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_BasePrinters_BasePrinterId",
                table: "AuditLogs",
                column: "BasePrinterId",
                principalTable: "BasePrinters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_Oids_OidId",
                table: "Histories",
                column: "OidId",
                principalTable: "Oids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
