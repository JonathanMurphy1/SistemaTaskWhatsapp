using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProgramaToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProgramaId",
                table: "AspNetUsers",
                column: "ProgramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Programa_ProgramaId",
                table: "AspNetUsers",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Programa_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "AspNetUsers");
        }
    }
}
