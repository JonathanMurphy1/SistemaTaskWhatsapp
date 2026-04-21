using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjusteProyectos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "Proyecto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "Proyecto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto",
                column: "ProgramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_Programa_ProgramaId",
                table: "Proyecto",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
