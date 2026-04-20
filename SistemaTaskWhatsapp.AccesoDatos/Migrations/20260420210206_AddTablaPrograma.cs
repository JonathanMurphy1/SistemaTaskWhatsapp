using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTablaPrograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "Proyecto",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Programa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programa", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Programa_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropTable(
                name: "Programa");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "Proyecto");
        }
    }
}
