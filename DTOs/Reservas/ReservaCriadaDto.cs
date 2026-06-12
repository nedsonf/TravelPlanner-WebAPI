namespace TravelPlanner.Api.DTOs.Reservas;

public class ReservaCriadaDto
{
    public int ReservaId { get; set; }
    public int DespesaId { get; set; }
    public int ViagemId { get; set; }
    public int? HospedagemId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string DespesaDescricao { get; set; } = string.Empty;
}
