namespace TravelPlanner.Api.DTOs.ExternalApis;

public record WeatherForecastItemDto(
    DateTime DataHora,
    double Temperatura,
    string Descricao,
    int Umidade);
