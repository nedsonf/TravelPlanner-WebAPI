namespace TravelPlanner.Api.DTOs.Destinos;

public class DestinoDto
{
    public int Id { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
