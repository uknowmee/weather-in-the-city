using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace WeatherInTheCity.Ai;

public interface IAiService
{
    IAsyncEnumerable<string> GetCityDescriptionAsync(GenerateDescriptionRequest request, CancellationToken token = default);
}

public class AiService : IAiService
{
    private readonly ILogger<AiService> _logger;
    private readonly ChatClient _chatClient;
    private readonly HybridCache _cache;
    
    private static string DescriptionForCityKey(Guid cityId) => $"description_{cityId.ToString()}";

    public AiService(ILoggerFactory loggerFactory, OpenAIClient openAiService, IOptions<OpenAiOptions> options, HybridCache cache)
    {
        _cache = cache;
        _logger = loggerFactory.CreateLogger<AiService>();
        _chatClient = openAiService.GetChatClient(options.Value.Model);
    }
    
    public async IAsyncEnumerable<string> GetCityDescriptionAsync(GenerateDescriptionRequest request, [EnumeratorCancellation] CancellationToken token = default)
    {
        _logger.LogInformation("Getting description for city with name {CityName} from cache", request.CityName);
        
        var description = await _cache.GetOrCreateAsync(
            DescriptionForCityKey(request.CityId), _ => ValueTask.FromResult(string.Empty),
            cancellationToken: token
        );

        if (string.IsNullOrWhiteSpace(description) is false)
        {
            _logger.LogInformation("Cache hit for city: {CityName}", request.CityName);
            yield return description;
        }
        else
        {
            _logger.LogInformation("Cache miss for city: {CityName}", request.CityName);
            _logger.LogInformation("Generating description for {CityName}", request.CityName);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Generate description about given city. It should be 3-5 sentences long."),
                new UserChatMessage(request.CityName)
            };

            var stringBuilder = new StringBuilder();
            var response = _chatClient.CompleteChatStreamingAsync(messages, cancellationToken: token);

            await foreach (var completionUpdate in response)
            {
                if (completionUpdate.ContentUpdate.Count <= 0) continue;

                stringBuilder.Append(completionUpdate.ContentUpdate[0].Text);
                yield return completionUpdate.ContentUpdate[0].Text;
            }

            description = stringBuilder.ToString();
            _logger.LogInformation("Generated description for {CityName}: {Description}", request.CityName, description);

            await _cache.SetAsync(
                DescriptionForCityKey(request.CityId),
                description,
                cancellationToken: token
            );
            _logger.LogInformation("Cached description for {CityName}", request.CityName);
        }
    }
}