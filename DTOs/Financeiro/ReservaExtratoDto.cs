namespace TravelPlanner.Api.DTOs.Financeiro;

public class ReservaExtratoDto
{
    public int Id { get; set; }
    public string Hospedagem { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public decimal TaxaPlataforma { get; set; }
    public decimal ValorLiquidoGuia { get; set; }
    public string? GuiaNome { get; set; }
    public int ViagemId { get; set; }
}
