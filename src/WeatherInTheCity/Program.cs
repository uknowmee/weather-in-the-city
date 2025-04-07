using WeatherInTheCity;
using WeatherInTheCity.CitiesDb;
using WeatherInTheCity.Cms;
using WeatherInTheCity.Framework;
using WeatherInTheCity.Mail;
using WeatherInTheCity.OpenAi;

var builder = WebApplication.CreateBuilder(args);

builder.AddBlazor()
    .AddFramework()
    .AddCmsService()
    .AddMailService()
    .AddAiService()
    .AddDatabase<CtxCitiesDb, CitiesDbOptions>();

var app = builder.Build();

app.UseBlazor()
    .UseFramework()
    .CreateDatabase<CtxCitiesDb>();

app.Run();