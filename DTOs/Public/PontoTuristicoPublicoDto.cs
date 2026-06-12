namespace TravelPlanner.Api.DTOs.Public;

public class PontoTuristicoPublicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? DestinoNome { get; set; }
}
