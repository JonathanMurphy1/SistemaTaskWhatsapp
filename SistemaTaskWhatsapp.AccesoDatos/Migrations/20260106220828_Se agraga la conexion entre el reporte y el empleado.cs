using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seagragalaconexionentreelreporteyelempleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpleadoId",
                table: "Reporte",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reporte_EmpleadoId",
                table: "Reporte",
                column: "EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reporte_Empleado_EmpleadoId",
                table: "Reporte",
                column: "EmpleadoId",
                principalTable: "Empleado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reporte_Empleado_EmpleadoId",
                table: "Reporte");

            migrationBuilder.DropIndex(
                name: "IX_Reporte_EmpleadoId",
                table: "Reporte");

            migrationBuilder.DropColumn(
                name: "EmpleadoId",
                table: "Reporte");
        }
    }
}
