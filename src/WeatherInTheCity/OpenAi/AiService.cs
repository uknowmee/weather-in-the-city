using System.Text;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace WeatherInTheCity.OpenAi;

public interface IAiService
{
    IAsyncEnumerable<string> GetCityDescriptionAsync(string cityName);
}

public class AiService : IAiService
{
    private readonly ILogger<AiService> _logger;
    private readonly ChatClient _chatClient;

    public AiService(ILoggerFactory loggerFactory, OpenAIClient openAiService, IOptions<OpenAiOptions> options)
    {
        _logger = loggerFactory.CreateLogger<AiService>();
        _chatClient = openAiService.GetChatClient(options.Value.Model);
    }
    
    public async IAsyncEnumerable<string> GetCityDescriptionAsync(string cityName)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("Generate description about given city. It should be 3-5 sentences long."),
            new UserChatMessage(cityName)
        };
        
        var stringBuilder = new StringBuilder();
        var response = _chatClient.CompleteChatStreamingAsync(messages);
        
        await foreach (var completionUpdate in response)
        {
            if (completionUpdate.ContentUpdate.Count <= 0) continue;
            
            stringBuilder.Append(completionUpdate.ContentUpdate[0].Text);
            yield return completionUpdate.ContentUpdate[0].Text;
        }
        
        _logger.LogInformation("Generated description for {CityName}: {Description}", cityName, stringBuilder.ToString());
    }
}