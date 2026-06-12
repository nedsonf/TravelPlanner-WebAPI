using System.Text.Json.Serialization;

namespace TravelPlanner.Api.DTOs.ExternalApis;

public class GeocodingResponse
{
    [JsonPropertyName("results")]
    public List<GeocodingResult>? Results { get; set; }
}

public class GeocodingResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("admin1")]
    public string? Admin1 { get; set; }
}

public record CidadeEncontradaDto(
    string Nome,
    string? Estado,
    string Pais,
    double Latitude,
    double Longitude);
