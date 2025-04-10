using WeatherInTheCity;
using WeatherInTheCity.Ai;
using WeatherInTheCity.Cities;
using WeatherInTheCity.Cms;
using WeatherInTheCity.Framework;
using WeatherInTheCity.Mail;
using WeatherInTheCity.Weather;

var builder = WebApplication.CreateBuilder(args);

builder.AddBlazor()
    .AddFramework()
    .AddCmsService()
    .AddMailService()
    .AddAiService()
    .AddWeatherService()
    .AddCitiesServices();

var app = builder.Build();

app.UseBlazor()
    .UseFramework()
    .UseCitiesServices();

app.Run();