using System.Net.Http.Headers;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace WeatherInTheCity.Cms;

public class HygraphOptions
{
    public const string GraphQlHttpClientKey = nameof(HygraphOptions);
    public string Endpoint { get; set; } = string.Empty;
    public string Pat { get; set; } = string.Empty;
}

public static class Extensions
{
    public static IHostApplicationBuilder AddCmsService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddKeyedSingleton<GraphQLHttpClient>(HygraphOptions.GraphQlHttpClientKey, (serviceProvider, key) =>
        {
            var hygraphOptions = new HygraphOptions();
            builder.Configuration.GetSection(nameof(HygraphOptions)).Bind(hygraphOptions);

            var httpClient = new HttpClient
            {
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", hygraphOptions.Pat) }
            };

            return new GraphQLHttpClient(hygraphOptions.Endpoint, new SystemTextJsonSerializer(), httpClient);
        });

        builder.Services.AddSingleton<ICmsService, CmsService>();

        return builder;
    }
}