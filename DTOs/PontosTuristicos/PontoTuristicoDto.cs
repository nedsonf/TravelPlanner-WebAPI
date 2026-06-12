namespace TravelPlanner.Api.DTOs.PontosTuristicos;

public class PontoTuristicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int DestinoId { get; set; }
    public string DestinoNome { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
