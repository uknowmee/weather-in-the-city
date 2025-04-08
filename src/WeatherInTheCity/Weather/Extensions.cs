namespace WeatherInTheCity.Weather;

public static class Extensions
{
    public static IHostApplicationBuilder AddWeatherService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
        {
            client.BaseAddress = new Uri("https://wttr.in/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });
        
        return builder;
    }
}