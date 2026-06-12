using TravelPlanner.Api.DTOs.ExternalApis;

namespace TravelPlanner.Api.DTOs.Roteiro;

public class RoteiroResumoDto
{
    public int ViagemId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public DestinoResumoDto Destino { get; set; } = new();
    public IReadOnlyList<WeatherForecastItemDto> PrevisaoClima { get; set; } = [];
    public IReadOnlyList<TouristPointDto> PontosTuristicosSugeridos { get; set; } = [];
}

public class DestinoResumoDto
{
    public int Id { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
