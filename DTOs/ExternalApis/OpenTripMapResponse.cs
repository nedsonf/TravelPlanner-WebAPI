using System.Text.Json.Serialization;

namespace TravelPlanner.Api.DTOs.ExternalApis;

public class OpenTripMapResponse
{
    [JsonPropertyName("features")]
    public List<OpenTripMapFeature> Features { get; set; } = [];
}

public class OpenTripMapFeature
{
    [JsonPropertyName("properties")]
    public OpenTripMapProperties Properties { get; set; } = new();

    [JsonPropertyName("geometry")]
    public OpenTripMapGeometry Geometry { get; set; } = new();
}

public class OpenTripMapProperties
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("kinds")]
    public string Kinds { get; set; } = string.Empty;
}

public class OpenTripMapGeometry
{
    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; set; } = [];
}
