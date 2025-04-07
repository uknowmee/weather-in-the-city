using Microsoft.Extensions.Options;
using OpenAI;

namespace WeatherInTheCity.OpenAi;

public class OpenAiOptions
{
    public string Model { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public static class Extensions
{
    public static IHostApplicationBuilder AddAiService(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection(nameof(OpenAiOptions)));
        builder.Services.AddSingleton(provider => new OpenAIClient(provider.GetRequiredService<IOptions<OpenAiOptions>>().Value.ApiKey));
        builder.Services.AddScoped<IAiService, AiService>();
        
        return builder;
    }
}