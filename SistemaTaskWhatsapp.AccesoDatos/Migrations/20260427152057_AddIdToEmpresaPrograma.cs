using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToEmpresaPrograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpresaPrograma",
                table: "EmpresaPrograma");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EmpresaPrograma",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpresaPrograma",
                table: "EmpresaPrograma",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaPrograma_EmpresaId_ProgramaId",
                table: "EmpresaPrograma",
                columns: new[] { "EmpresaId", "ProgramaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpresaPrograma",
                table: "EmpresaPrograma");

            migrationBuilder.DropIndex(
                name: "IX_EmpresaPrograma_EmpresaId_ProgramaId",
                table: "EmpresaPrograma");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmpresaPrograma");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpresaPrograma",
                table: "EmpresaPrograma",
                columns: new[] { "EmpresaId", "ProgramaId" });
        }
    }
}
