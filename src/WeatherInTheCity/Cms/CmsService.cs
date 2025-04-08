using GraphQL;
using GraphQL.Client.Http;

namespace WeatherInTheCity.Cms;

public interface ICmsService
{
    public Task<About> GetAppAbout();
    public Task<IEnumerable<Project>> GetProjects();
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

    public async Task<About> GetAppAbout()
    {
        _logger.LogInformation("Fetching app about from CMS");

        var request = new GraphQLRequest
        {
            Query = Queries.GetAppAbout,
            Variables = new { name = "Weather In The City" }
        };
        var response = await _hygraphClient.SendQueryAsync<Queries.GetAppAboutResponse>(request);
        var about = response.Data.AppAbout;

        _logger.LogInformation("Fetched app about from CMS");

        return about;
    }

    public async Task<IEnumerable<Project>> GetProjects()
    {
        _logger.LogInformation("Fetching projects from CMS");

        var request = new GraphQLRequest { Query = Queries.GetProjects };
        var response = await _hygraphClient.SendQueryAsync<Queries.GetProjectsResponse>(request);
        var projects = response.Data.Projects;

        _logger.LogInformation("Fetched {ProjectCount} projects from CMS", projects.Count);

        return projects.OrderBy(p => p.DisplayId);
    }
}