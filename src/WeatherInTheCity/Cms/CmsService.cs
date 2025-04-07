using GraphQL.Client.Http;

namespace WeatherInTheCity.Cms;

public interface ICmsService
{
}

public class CmsService : ICmsService
{
    private readonly ILogger<CmsService> _logger;
    private readonly GraphQLHttpClient _hygraphClient;

    public CmsService(ILoggerFactory loggerFactory, [FromKeyedServices(HygraphOptions.GraphQlHttpClientKey)] GraphQLHttpClient hygraphClient)
    {
        _logger = loggerFactory.CreateLogger<CmsService>();
        _hygraphClient = hygraphClient;
    }
}