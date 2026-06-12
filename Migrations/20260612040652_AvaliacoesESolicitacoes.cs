using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Api.Migrations
{
    /// <inheritdoc />
    public partial class AvaliacoesESolicitacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViajanteId",
                table: "Pacotes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AvaliacoesGuias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuiaId = table.Column<int>(type: "int", nullable: false),
                    ViajanteId = table.Column<int>(type: "int", nullable: false),
                    ReservaId = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesGuias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacoesGuias_Guias_GuiaId",
                        column: x => x.GuiaId,
                        principalTable: "Guias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvaliacoesGuias_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvaliacoesGuias_Viajantes_ViajanteId",
                        column: x => x.ViajanteId,
                        principalTable: "Viajantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitacoesPacote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuiaId = table.Column<int>(type: "int", nullable: false),
                    ViajanteId = table.Column<int>(type: "int", nullable: false),
                    DestinoId = table.Column<int>(type: "int", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoriaHotel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PacoteId = table.Column<int>(type: "int", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesPacote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesPacote_Destinos_DestinoId",
                        column: x => x.DestinoId,
                        principalTable: "Destinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesPacote_Guias_GuiaId",
                        column: x => x.GuiaId,
                        principalTable: "Guias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesPacote_Pacotes_PacoteId",
                        column: x => x.PacoteId,
                        principalTable: "Pacotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesPacote_Viajantes_ViajanteId",
                        column: x => x.ViajanteId,
                        principalTable: "Viajantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pacotes_ViajanteId",
                table: "Pacotes",
                column: "ViajanteId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesGuias_GuiaId",
                table: "AvaliacoesGuias",
                column: "GuiaId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesGuias_ReservaId",
                table: "AvaliacoesGuias",
                column: "ReservaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesGuias_ViajanteId",
                table: "AvaliacoesGuias",
                column: "ViajanteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesPacote_DestinoId",
                table: "SolicitacoesPacote",
                column: "DestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesPacote_GuiaId",
                table: "SolicitacoesPacote",
                column: "GuiaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesPacote_PacoteId",
                table: "SolicitacoesPacote",
                column: "PacoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesPacote_ViajanteId",
                table: "SolicitacoesPacote",
                column: "ViajanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pacotes_Viajantes_ViajanteId",
                table: "Pacotes",
                column: "ViajanteId",
                principalTable: "Viajantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pacotes_Viajantes_ViajanteId",
                table: "Pacotes");

            migrationBuilder.DropTable(
                name: "AvaliacoesGuias");

            migrationBuilder.DropTable(
                name: "SolicitacoesPacote");

            migrationBuilder.DropIndex(
                name: "IX_Pacotes_ViajanteId",
                table: "Pacotes");

            migrationBuilder.DropColumn(
                name: "ViajanteId",
                table: "Pacotes");
        }
    }
}
