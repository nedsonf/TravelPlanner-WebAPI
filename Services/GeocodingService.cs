using System.Text.Json;
using TravelPlanner.Api.DTOs.ExternalApis;

namespace TravelPlanner.Api.Services;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeocodingService> _logger;

    public GeocodingService(HttpClient httpClient, ILogger<GeocodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CidadeEncontradaDto>> BuscarCidadesAsync(string nome)
    {
        try
        {
            var url = $"v1/search?name={Uri.EscapeDataString(nome)}&count=8&language=pt&format=json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Geocoding retornou status {StatusCode} para busca '{Nome}'",
                    (int)response.StatusCode, nome);
                return [];
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<GeocodingResponse>(content);

            if (apiResponse?.Results is null || apiResponse.Results.Count == 0)
                return [];

            return apiResponse.Results
                .Select(r => new CidadeEncontradaDto(
                    Nome: r.Name,
                    Estado: r.Admin1,
                    Pais: r.Country ?? "Desconhecido",
                    Latitude: r.Latitude,
                    Longitude: r.Longitude))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consultar API de geocoding para '{Nome}'", nome);
            return [];
        }
    }
}
