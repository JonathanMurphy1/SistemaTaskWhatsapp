using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeagreagalatablaReporte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reporte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TareaId = table.Column<int>(type: "int", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inconvenientes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentarioEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reporte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reporte_Tarea_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reporte_TareaId",
                table: "Reporte",
                column: "TareaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reporte");
        }
    }
}
