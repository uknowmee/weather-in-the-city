namespace WeatherInTheCity.OpenAi;

public record GenerateDescriptionRequest(Guid CityId, string CityName);