using MudBlazor.Services;
using WeatherInTheCity.Components;

namespace WeatherInTheCity;

public static partial class Extensions
{
    public static WebApplicationBuilder AddBlazor(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddMudServices();

        return builder;
    }

    public static WebApplication UseBlazor(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
                    }
                    else
                    {
                        context.Response.Redirect("/error");
                    }
                });
            });

            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(WeatherInTheCity.Client._Imports).Assembly);

        app.UseAntiforgery();

        return app;
    }
}