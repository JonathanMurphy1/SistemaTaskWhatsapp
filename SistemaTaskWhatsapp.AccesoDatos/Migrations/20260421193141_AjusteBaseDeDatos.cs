using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjusteBaseDeDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa");

            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "Proyecto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProgramaId",
                table: "Empresa",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompaniesId",
                table: "Empresa",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto",
                column: "ProgramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Programa_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "Proyecto");

            migrationBuilder.AlterColumn<int>(
                name: "ProgramaId",
                table: "Empresa",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CompaniesId",
                table: "Empresa",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id");
        }
    }
}
