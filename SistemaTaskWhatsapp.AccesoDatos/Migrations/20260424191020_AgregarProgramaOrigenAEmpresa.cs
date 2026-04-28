using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProgramaOrigenAEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaPrograma_Programa_ProgramaId",
                table: "EmpresaPrograma");

            migrationBuilder.AddColumn<int>(
                name: "ProgramaOrigenId",
                table: "Empresa",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_ProgramaOrigenId",
                table: "Empresa",
                column: "ProgramaOrigenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresa_Programa_ProgramaOrigenId",
                table: "Empresa",
                column: "ProgramaOrigenId",
                principalTable: "Programa",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaPrograma_Programa_ProgramaId",
                table: "EmpresaPrograma",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresa_Programa_ProgramaOrigenId",
                table: "Empresa");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaPrograma_Programa_ProgramaId",
                table: "EmpresaPrograma");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_ProgramaOrigenId",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "ProgramaOrigenId",
                table: "Empresa");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaPrograma_Programa_ProgramaId",
                table: "EmpresaPrograma",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
