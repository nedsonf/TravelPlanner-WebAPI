namespace TravelPlanner.Api.DTOs.Viagens;

public class ViagemDto
{
    public int Id { get; set; }
    public int DestinoId { get; set; }
    public string DestinoNome { get; set; } = string.Empty;
    public string DestinoPais { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string Status { get; set; } = string.Empty;
}
