using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxaPlataforma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuiaId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PacoteId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxaPlataforma",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorLiquidoGuia",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_GuiaId",
                table: "Reservas",
                column: "GuiaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_PacoteId",
                table: "Reservas",
                column: "PacoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Guias_GuiaId",
                table: "Reservas",
                column: "GuiaId",
                principalTable: "Guias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Pacotes_PacoteId",
                table: "Reservas",
                column: "PacoteId",
                principalTable: "Pacotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Guias_GuiaId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Pacotes_PacoteId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_GuiaId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_PacoteId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "GuiaId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "PacoteId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "TaxaPlataforma",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ValorLiquidoGuia",
                table: "Reservas");
        }
    }
}
