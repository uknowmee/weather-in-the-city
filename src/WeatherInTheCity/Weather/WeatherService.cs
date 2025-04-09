using System.Text.Json;
using Microsoft.Extensions.Caching.Hybrid;

namespace WeatherInTheCity.Weather;

public interface IWeatherService
{
    Task<CurrentCondition> GetWeatherAsync(GenerateWeatherRequest request, CancellationToken token);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly HybridCache _cache;
    private readonly ILogger<WeatherService> _logger;
    
    private static string WeatherForCityKey(Guid cityId) => $"weather_{cityId.ToString()}";

    public WeatherService(HttpClient httpClient, ILoggerFactory loggerFactory, HybridCache cache)
    {
        _logger = loggerFactory.CreateLogger<WeatherService>();
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<CurrentCondition> GetWeatherAsync(GenerateWeatherRequest request, CancellationToken token)
    {
        _logger.LogInformation("Getting weather for city with name {CityName} from cache", request.CityName);
        
        var currentCondition = await _cache.GetOrCreateAsync(
            WeatherForCityKey(request.CityId),
            async ct =>
            {
                _logger.LogInformation("Cache miss for city: {CityName}", request.CityName);
                _logger.LogInformation("Fetching weather data for city: {CityName}", request.CityName);
        
                var response = await _httpClient.GetAsync($"{request.CityName}?format=j1", ct);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(ct);
                var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content)
                                      ?? throw new InvalidOperationException("Failed to deserialize weather response");

                _logger.LogInformation("Weather data fetched successfully");
                
                return weatherResponse.CurrentConditions[0];
            },
            cancellationToken: token
        );
        
        _logger.LogInformation("Weather data for city {CityName} retrieved successfully: {@CurrentCondition}", request.CityName, currentCondition);
        
        return currentCondition;
    }
}