using System.Text.Json;
using Microsoft.Extensions.Options;
using TravelPlanner.Api.Configuration;
using TravelPlanner.Api.DTOs.ExternalApis;

namespace TravelPlanner.Api.Services;

public class OpenTripMapService : IOpenTripMapService
{
    private readonly HttpClient _httpClient;
    private readonly OpenTripMapSettings _settings;
    private readonly ILogger<OpenTripMapService> _logger;

    public OpenTripMapService(
        HttpClient httpClient,
        IOptions<OpenTripMapSettings> settings,
        ILogger<OpenTripMapService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TouristPointDto>> GetTouristPointsAsync(
        double latitude,
        double longitude,
        int radiusMeters = 5000)
    {
        try
        {
            var url = $"places/radius?radius={radiusMeters}&lon={longitude}&lat={latitude}&apikey={_settings.ApiKey}&limit=20";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "OpenTripMap retornou status {StatusCode} para lat={Latitude}, lon={Longitude}",
                    (int)response.StatusCode, latitude, longitude);
                return [];
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<OpenTripMapResponse>(content);

            if (apiResponse?.Features is null || apiResponse.Features.Count == 0)
                return [];

            return apiResponse.Features
                .Where(f => f.Geometry.Coordinates.Count >= 2)
                .Select(f => new TouristPointDto(
                    Nome: f.Properties.Name,
                    Categoria: f.Properties.Kinds,
                    Longitude: f.Geometry.Coordinates[0],
                    Latitude: f.Geometry.Coordinates[1]))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Falha ao consultar OpenTripMap para lat={Latitude}, lon={Longitude}",
                latitude, longitude);
            return [];
        }
    }
}
