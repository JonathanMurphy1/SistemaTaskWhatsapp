using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InicialNuevaEstructura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubtaskId",
                table: "Tarea",
                newName: "IdExterno");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Proyecto",
                newName: "IdExterno");

            migrationBuilder.RenameColumn(
                name: "CompaniesId",
                table: "Empresa",
                newName: "IdExterno");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUsers",
                newName: "IdExterno");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Tarea",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "Proyecto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tarea_IdExterno_ProyectoId",
                table: "Tarea",
                columns: new[] { "IdExterno", "ProyectoId" },
                unique: true,
                filter: "[IdExterno] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tarea_Nombre_ProyectoId",
                table: "Tarea",
                columns: new[] { "Nombre", "ProyectoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_IdExterno_ProgramaId",
                table: "Proyecto",
                columns: new[] { "IdExterno", "ProgramaId" },
                unique: true,
                filter: "[IdExterno] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto",
                column: "ProgramaId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_IdExterno_ProgramaId",
                table: "Empresa",
                columns: new[] { "IdExterno", "ProgramaId" },
                unique: true,
                filter: "[IdExterno] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdExterno_ProgramaId",
                table: "AspNetUsers",
                columns: new[] { "IdExterno", "ProgramaId" },
                unique: true,
                filter: "[IdExterno] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProgramaId",
                table: "AspNetUsers",
                column: "ProgramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Programa_ProgramaId",
                table: "AspNetUsers",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_AspNetUsers_Programa_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Programa_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_Tarea_IdExterno_ProyectoId",
                table: "Tarea");

            migrationBuilder.DropIndex(
                name: "IX_Tarea_Nombre_ProyectoId",
                table: "Tarea");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_IdExterno_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_IdExterno_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdExterno_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IdExterno",
                table: "Tarea",
                newName: "SubtaskId");

            migrationBuilder.RenameColumn(
                name: "IdExterno",
                table: "Proyecto",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "IdExterno",
                table: "Empresa",
                newName: "CompaniesId");

            migrationBuilder.RenameColumn(
                name: "IdExterno",
                table: "AspNetUsers",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Tarea",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
