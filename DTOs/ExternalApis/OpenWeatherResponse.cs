using System.Text.Json.Serialization;

namespace TravelPlanner.Api.DTOs.ExternalApis;

public class OpenWeatherResponse
{
    [JsonPropertyName("list")]
    public List<OpenWeatherForecastEntry> List { get; set; } = [];
}

public class OpenWeatherForecastEntry
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public OpenWeatherMain Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<OpenWeatherCondition> Weather { get; set; } = [];
}

public class OpenWeatherMain
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class OpenWeatherCondition
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
