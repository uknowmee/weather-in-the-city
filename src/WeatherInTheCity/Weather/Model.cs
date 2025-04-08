using System.Text.Json.Serialization;

namespace WeatherInTheCity.Weather;

public class WeatherResponse
{
    [JsonPropertyName("current_condition")]
    public CurrentCondition[] CurrentConditions { get; set; } = [];
}

public class CurrentCondition
{
    public DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.UtcNow;
    [JsonPropertyName("localObsDateTime")] public string LocalObservationTime { get; set; } = string.Empty;
    [JsonPropertyName("temp_C")] public string TempC { get; set; } = string.Empty;
    [JsonPropertyName("weatherDesc")] public WeatherDescription[] WeatherDescription { get; set; } = [];
    [JsonPropertyName("humidity")] public string Humidity { get; set; } = string.Empty;
    [JsonPropertyName("windspeedKmph")] public string WindSpeedKmph { get; set; } = string.Empty;
    [JsonPropertyName("FeelsLikeC")] public string FeelsLikeC { get; set; } = string.Empty;
}

public class WeatherDescription
{
    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
}