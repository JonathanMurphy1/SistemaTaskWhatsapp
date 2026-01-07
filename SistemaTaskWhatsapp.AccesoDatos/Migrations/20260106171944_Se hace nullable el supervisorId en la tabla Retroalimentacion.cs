using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SehacenullableelsupervisorIdenlatablaRetroalimentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Retroalimentacion_Supervisor_SupervisorId",
                table: "Retroalimentacion");

            migrationBuilder.AlterColumn<int>(
                name: "SupervisorId",
                table: "Retroalimentacion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Retroalimentacion_Supervisor_SupervisorId",
                table: "Retroalimentacion",
                column: "SupervisorId",
                principalTable: "Supervisor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Retroalimentacion_Supervisor_SupervisorId",
                table: "Retroalimentacion");

            migrationBuilder.AlterColumn<int>(
                name: "SupervisorId",
                table: "Retroalimentacion",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Retroalimentacion_Supervisor_SupervisorId",
                table: "Retroalimentacion",
                column: "SupervisorId",
                principalTable: "Supervisor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
