namespace TravelPlanner.Api.DTOs.Public;

public class DestinoPublicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int TotalPontosTuristicos { get; set; }
    public List<string> PontosDestaque { get; set; } = [];
}
