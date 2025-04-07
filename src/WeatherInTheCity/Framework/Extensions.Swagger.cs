namespace WeatherInTheCity.Framework;

public static partial class Extensions
{
    private static IHostApplicationBuilder AddSwagger(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();
        return builder;
    }

    private static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
        if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            SwaggerBuilderExtensions.UseSwagger(app).UseSwaggerUI();
        }

        return app;
    }
}