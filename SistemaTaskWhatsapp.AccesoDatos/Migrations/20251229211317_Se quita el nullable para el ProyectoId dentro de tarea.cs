using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SequitaelnullableparaelProyectoIddentrodetarea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarea_Proyecto_ProyectoId",
                table: "Tarea");

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "Tarea",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarea_Proyecto_ProyectoId",
                table: "Tarea",
                column: "ProyectoId",
                principalTable: "Proyecto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarea_Proyecto_ProyectoId",
                table: "Tarea");

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "Tarea",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarea_Proyecto_ProyectoId",
                table: "Tarea",
                column: "ProyectoId",
                principalTable: "Proyecto",
                principalColumn: "Id");
        }
    }
}
