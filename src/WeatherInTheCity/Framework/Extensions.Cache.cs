using Microsoft.Extensions.Caching.Hybrid;

namespace WeatherInTheCity.Framework;

public class CacheOptions
{
    public int ExpirationTime { get; set; } = 2;
}

public static partial class Extensions
{
    private static IHostApplicationBuilder AddHybridCache(this IHostApplicationBuilder builder)
    {
        var cacheOptions = new CacheOptions();
        builder.Configuration.GetSection(nameof(CacheOptions)).Bind(cacheOptions);

        builder.Services.AddHybridCache(options =>
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(cacheOptions.ExpirationTime)
            }
        );

        return builder;
    }
}