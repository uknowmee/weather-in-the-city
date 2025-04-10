using WeatherInTheCity.Framework;

namespace WeatherInTheCity.Cities;

public static class Extensions
{
    public static IHostApplicationBuilder AddCitiesServices(this IHostApplicationBuilder builder)
    {
        builder.AddDatabase<CtxCitiesDb, CitiesDbOptions>();
        builder.Services.AddScoped<IResetCitiesDbService, ResetCitiesDbService>();
        return builder;
    }

    public static WebApplication UseCitiesServices(this WebApplication app)
    {
        app.MapPost("/reset/database", async (IResetCitiesDbService resetCitiesDbService, CancellationToken cancellationToken)
            => await resetCitiesDbService.ResetDatabaseAsync(cancellationToken)
        );
        app.CreateDatabase<CtxCitiesDb>();

        return app;
    }
}