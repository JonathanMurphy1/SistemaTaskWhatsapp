using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeAgregaElCampoNombreAlReporte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Reporte",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Reporte");
        }
    }
}
