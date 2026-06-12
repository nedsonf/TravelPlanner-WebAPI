using TravelPlanner.Api.DTOs.ExternalApis;

namespace TravelPlanner.Api.Services;

public interface IOpenTripMapService
{
    Task<IReadOnlyList<TouristPointDto>> GetTouristPointsAsync(
        double latitude,
        double longitude,
        int radiusMeters = 5000);
}
