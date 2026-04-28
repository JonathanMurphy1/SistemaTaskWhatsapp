using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTaskWhatsapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RediseñoEmpresasProgramas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Programa_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_IdExterno_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_Nombre_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_ProgramaId",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdExterno_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "ProgramaId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "EmpresaPrograma",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    ProgramaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaPrograma", x => new { x.EmpresaId, x.ProgramaId });
                    table.ForeignKey(
                        name: "FK_EmpresaPrograma_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpresaPrograma_Programa_ProgramaId",
                        column: x => x.ProgramaId,
                        principalTable: "Programa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_Nombre",
                table: "Empresa",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdExterno",
                table: "AspNetUsers",
                column: "IdExterno",
                filter: "[IdExterno] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaPrograma_ProgramaId",
                table: "EmpresaPrograma",
                column: "ProgramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EmpresaPrograma");

            migrationBuilder.DropIndex(
                name: "IX_Empresa_Nombre",
                table: "Empresa");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdExterno",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "ProgramaId",
                table: "Empresa",
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
                name: "IX_Empresa_IdExterno_ProgramaId",
                table: "Empresa",
                columns: new[] { "IdExterno", "ProgramaId" },
                unique: true,
                filter: "[IdExterno] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_Nombre_ProgramaId",
                table: "Empresa",
                columns: new[] { "Nombre", "ProgramaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_ProgramaId",
                table: "Empresa",
                column: "ProgramaId");

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
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Programa_ProgramaId",
                table: "AspNetUsers",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Empresa_Programa_ProgramaId",
                table: "Empresa",
                column: "ProgramaId",
                principalTable: "Programa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
