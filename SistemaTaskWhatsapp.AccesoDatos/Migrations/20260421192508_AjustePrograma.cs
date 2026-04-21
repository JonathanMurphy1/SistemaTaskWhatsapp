using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjustePrograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Programa_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "Proyecto");

            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "Empresa",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_ProgramaId",
                table: "Empresa",
                column: "ProgramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "Empresa");

            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "Proyecto",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto",
                column: "ProgramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_Programa_ProgramaId",
                table: "Proyecto",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id");
        }
    }
}
