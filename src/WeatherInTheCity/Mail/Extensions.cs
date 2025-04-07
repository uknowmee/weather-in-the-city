namespace WeatherInTheCity.Mail;

public class SendGridOptions
{
    public string Key { get; set; } = string.Empty;
    public string EmailFrom { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
}

public static class Extensions
{
    public static IHostApplicationBuilder AddMailService(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection(nameof(SendGridOptions)));
        builder.Services.AddSingleton<IMailService, MailService>();

        return builder;
    }
}