using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectIdToProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Proyecto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompaniesId",
                table: "Empresa",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "CompaniesId",
                table: "Empresa");
        }
    }
}
