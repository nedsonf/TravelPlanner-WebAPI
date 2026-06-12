namespace TravelPlanner.Api.DTOs.Roteiro;

public class RoteiroDiarioDto
{
    public int Id { get; set; }
    public int ViagemId { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
