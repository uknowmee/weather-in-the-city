using WeatherInTheCity;
using WeatherInTheCity.CitiesDb;
using WeatherInTheCity.Cms;
using WeatherInTheCity.Framework;
using WeatherInTheCity.Mail;
using WeatherInTheCity.OpenAi;
using WeatherInTheCity.Weather;

var builder = WebApplication.CreateBuilder(args);

builder.AddBlazor()
    .AddFramework()
    .AddCmsService()
    .AddMailService()
    .AddAiService()
    .AddWeatherService()
    .AddDatabase<CtxCitiesDb, CitiesDbOptions>();

var app = builder.Build();

app.UseBlazor()
    .UseFramework()
    .CreateDatabase<CtxCitiesDb>();

app.Run();