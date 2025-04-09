using System.Text.Json.Serialization;

namespace WeatherInTheCity.Weather;

public record GenerateWeatherRequest(Guid CityId, string CityName);

public record WeatherResponse([property: JsonPropertyName("current_condition")] CurrentCondition[] CurrentConditions)
{
    public WeatherResponse() : this([]) { }
}

public record CurrentCondition(
    [property: JsonPropertyName("localObsDateTime")] string LocalObservationTime,
    [property: JsonPropertyName("temp_C")] string TempC,
    [property: JsonPropertyName("weatherDesc")] WeatherDescription[] WeatherDescription,
    [property: JsonPropertyName("humidity")] string Humidity,
    [property: JsonPropertyName("windspeedKmph")] string WindSpeedKmph,
    [property: JsonPropertyName("FeelsLikeC")] string FeelsLikeC
)
{
    public CurrentCondition() : this(string.Empty, string.Empty, [], string.Empty, string.Empty, string.Empty) { }

    public DateTimeOffset LastUpdate { get; init; } = DateTimeOffset.UtcNow;
}

public record WeatherDescription([property: JsonPropertyName("value")] string Value)
{
    public WeatherDescription() : this(string.Empty) { }
}