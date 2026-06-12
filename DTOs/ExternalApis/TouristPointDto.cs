namespace TravelPlanner.Api.DTOs.ExternalApis;

public record TouristPointDto(
    string Nome,
    string Categoria,
    double Latitude,
    double Longitude);
