using TravelPlanner.Api.DTOs.ExternalApis;

namespace TravelPlanner.Api.Services;

public interface IOpenWeatherService
{
    Task<IReadOnlyList<WeatherForecastItemDto>?> GetFiveDayForecastAsync(double latitude, double longitude);
}
