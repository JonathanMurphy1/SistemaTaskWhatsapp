using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class MensajeFestivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MensajeDiaFestivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MensajeId = table.Column<int>(type: "int", nullable: false),
                    DiaFestivoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeDiaFestivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajeDiaFestivo_DiaFestivo_DiaFestivoId",
                        column: x => x.DiaFestivoId,
                        principalTable: "DiaFestivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MensajeDiaFestivo_Mensaje_MensajeId",
                        column: x => x.MensajeId,
                        principalTable: "Mensaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MensajeDiaFestivo_DiaFestivoId",
                table: "MensajeDiaFestivo",
                column: "DiaFestivoId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajeDiaFestivo_MensajeId",
                table: "MensajeDiaFestivo",
                column: "MensajeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensajeDiaFestivo");
        }
    }
}
