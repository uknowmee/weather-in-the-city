using System.Text.Json;

namespace WeatherInTheCity.Weather;

public interface IWeatherService
{
    Task<CurrentCondition> GetWeatherAsync(string city, CancellationToken token);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<WeatherService>();
        _httpClient = httpClient;
    }

    public async Task<CurrentCondition> GetWeatherAsync(string city, CancellationToken token)
    {
        _logger.LogInformation("Fetching weather data for city: {City}", city);
        
        var response = await _httpClient.GetAsync($"{city}?format=j1", token);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(token);
        var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content)
                              ?? throw new InvalidOperationException("Failed to deserialize weather response");

        _logger.LogInformation("Weather data fetched successfully: {WeatherData}", weatherResponse);
        
        return weatherResponse.CurrentConditions[0];
    }
}