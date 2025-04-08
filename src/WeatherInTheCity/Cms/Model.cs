using System.Text.Json.Serialization;

namespace WeatherInTheCity.Cms;

public record About(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] Content Content,
    [property: JsonPropertyName("features")] Content? KeyFeatures
);

public record Project(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("year")] int Year,
    [property: JsonPropertyName("gitLink")] string GitLink,
    [property: JsonPropertyName("content")] Content Content,
    [property: JsonPropertyName("img")] Img? Img,
    [property: JsonPropertyName("displayId")] int DisplayId
);

public record Content([property: JsonPropertyName("html")] string Html);

public record Img(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("placeholder")] string Placeholder
);