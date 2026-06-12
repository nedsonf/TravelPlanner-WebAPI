using System.Text.Json;
using Microsoft.Extensions.Options;
using TravelPlanner.Api.Configuration;
using TravelPlanner.Api.DTOs.ExternalApis;

namespace TravelPlanner.Api.Services;

public class OpenWeatherService : IOpenWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherSettings _settings;
    private readonly ILogger<OpenWeatherService> _logger;

    public OpenWeatherService(
        HttpClient httpClient,
        IOptions<OpenWeatherSettings> settings,
        ILogger<OpenWeatherService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<WeatherForecastItemDto>?> GetFiveDayForecastAsync(
        double latitude,
        double longitude)
    {
        try
        {
            var url = $"forecast?lat={latitude}&lon={longitude}&appid={_settings.ApiKey}&units=metric&lang=pt_br";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "OpenWeather retornou status {StatusCode} para lat={Latitude}, lon={Longitude}",
                    (int)response.StatusCode, latitude, longitude);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(content);

            if (apiResponse?.List is null || apiResponse.List.Count == 0)
                return null;

            return apiResponse.List
                .Select(entry => new WeatherForecastItemDto(
                    DataHora: DateTimeOffset.FromUnixTimeSeconds(entry.Dt).UtcDateTime,
                    Temperatura: entry.Main.Temp,
                    Descricao: entry.Weather.FirstOrDefault()?.Description ?? string.Empty,
                    Umidade: entry.Main.Humidity))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Falha ao consultar OpenWeather para lat={Latitude}, lon={Longitude}",
                latitude, longitude);
            return null;
        }
    }
}
