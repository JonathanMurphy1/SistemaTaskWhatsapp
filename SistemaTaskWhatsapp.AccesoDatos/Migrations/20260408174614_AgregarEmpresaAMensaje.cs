using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEmpresaAMensaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Mensaje",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mensaje_EmpresaId",
                table: "Mensaje",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensaje_Empresa_EmpresaId",
                table: "Mensaje",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensaje_Empresa_EmpresaId",
                table: "Mensaje");

            migrationBuilder.DropIndex(
                name: "IX_Mensaje_EmpresaId",
                table: "Mensaje");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Mensaje");
        }
    }
}
