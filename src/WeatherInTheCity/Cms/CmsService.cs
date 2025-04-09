using GraphQL;
using GraphQL.Client.Http;
using Microsoft.Extensions.Caching.Hybrid;

namespace WeatherInTheCity.Cms;

public interface ICmsService
{
    public Task<About> GetAppAboutAsync(CancellationToken token);
    public Task<IEnumerable<Project>> GetProjectsAsync(CancellationToken token);
}

public class CmsService : ICmsService
{
    private readonly ILogger<CmsService> _logger;
    private readonly GraphQLHttpClient _hygraphClient;
    private readonly HybridCache _cache;

    private static string AppAboutKey => "about";
    private static string ProjectsKey => "projects";

    public CmsService(ILoggerFactory loggerFactory, [FromKeyedServices(HygraphOptions.GraphQlHttpClientKey)] GraphQLHttpClient hygraphClient, HybridCache cache)
    {
        _logger = loggerFactory.CreateLogger<CmsService>();
        _hygraphClient = hygraphClient;
        _cache = cache;
    }

    public async Task<About> GetAppAboutAsync(CancellationToken token)
    {
        _logger.LogInformation("Getting app about from cache");

        var about = await _cache.GetOrCreateAsync(
            AppAboutKey,
            async ct =>
            {
                _logger.LogInformation("Cache miss for app about, fetching from CMS");

                var request = new GraphQLRequest
                {
                    Query = Queries.GetAppAbout,
                    Variables = new { name = "Weather In The City" }
                };
                var response = await _hygraphClient.SendQueryAsync<Queries.GetAppAboutResponse>(request, ct);
                var about = response.Data.AppAbout;

                _logger.LogInformation("Fetched app about from CMS");

                return about;
            },
            cancellationToken: token
        );

        _logger.LogInformation("App about retrieved successfully");

        return about;
    }

    public async Task<IEnumerable<Project>> GetProjectsAsync(CancellationToken token)
    {
        _logger.LogInformation("Getting projects from cache");

        var projects = await _cache.GetOrCreateAsync(
            ProjectsKey,
            async ct =>
            {
                _logger.LogInformation("Cache miss for projects, fetching from CMS");

                var request = new GraphQLRequest { Query = Queries.GetProjects };
                var response = await _hygraphClient.SendQueryAsync<Queries.GetProjectsResponse>(request, ct);
                var projects = response.Data.Projects;

                _logger.LogInformation("Fetched {ProjectCount} projects from CMS", projects.Count);

                return projects.OrderBy(p => p.DisplayId).ToList();
            },
            cancellationToken: token
        );

        _logger.LogInformation("Projects retrieved successfully");

        return projects;
    }
}