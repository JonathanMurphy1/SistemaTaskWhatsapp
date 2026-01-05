using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seactualizalabasededatosparaquetengancongruencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleado_Tarea_TareaId",
                table: "Empleado");

            migrationBuilder.DropIndex(
                name: "IX_Empleado_TareaId",
                table: "Empleado");

            migrationBuilder.DropColumn(
                name: "TareaId",
                table: "Empleado");

            migrationBuilder.AddColumn<int>(
                name: "EmpleadoId",
                table: "Tarea",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Supervisor",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Empleado",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tarea_EmpleadoId",
                table: "Tarea",
                column: "EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarea_Empleado_EmpleadoId",
                table: "Tarea",
                column: "EmpleadoId",
                principalTable: "Empleado",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarea_Empleado_EmpleadoId",
                table: "Tarea");

            migrationBuilder.DropIndex(
                name: "IX_Tarea_EmpleadoId",
                table: "Tarea");

            migrationBuilder.DropColumn(
                name: "EmpleadoId",
                table: "Tarea");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Supervisor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Empleado",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TareaId",
                table: "Empleado",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_TareaId",
                table: "Empleado",
                column: "TareaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empleado_Tarea_TareaId",
                table: "Empleado",
                column: "TareaId",
                principalTable: "Tarea",
                principalColumn: "Id");
        }
    }
}
