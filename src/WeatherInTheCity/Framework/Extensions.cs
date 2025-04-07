namespace WeatherInTheCity.Framework;

public static partial class Extensions
{
    public static WebApplicationBuilder AddFramework(this WebApplicationBuilder builder)
    {
        builder.AddLogging();
        ((IHostApplicationBuilder)builder).AddFramework();
        return builder;
    }

    public static IHostApplicationBuilder AddFramework(this IHostApplicationBuilder builder)
    {
        builder.AddConfiguration();

        builder.AddSwagger()
            .AddLogger()
            .AddHybridCache();

        return builder;
    }

    public static WebApplication UseFramework(this WebApplication app)
    {
        app.UseContextLogger()
            .UseSwagger();

        return app;
    }

    private static void AddConfiguration(this IHostApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}