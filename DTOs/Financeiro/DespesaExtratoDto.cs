namespace TravelPlanner.Api.DTOs.Financeiro;

public class DespesaExtratoDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int ViagemId { get; set; }
    public int? ReservaId { get; set; }
}
