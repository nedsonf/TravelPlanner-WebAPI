namespace TravelPlanner.Api.DTOs.Pacotes;

public class PacoteReservadoDto
{
    public int PacoteId { get; set; }
    public int ViagemId { get; set; }
    public int? ReservaId { get; set; }
    public int? DespesaId { get; set; }
    public string DestinoNome { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public int DiasRoteiro { get; set; }
}
