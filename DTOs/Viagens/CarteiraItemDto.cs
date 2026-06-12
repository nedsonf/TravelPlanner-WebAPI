namespace TravelPlanner.Api.DTOs.Viagens;

public class CarteiraItemDto
{
    public int ViagemId { get; set; }
    public string Destino { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? PacoteTitulo { get; set; }
}
