using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using WeatherInTheCity.Cities;
using WeatherInTheCity.Mail;

namespace WeatherInTheCity.Components.Pages;

public partial class Export
{
    private string _email = string.Empty;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, "en-US")] private static partial Regex ValidEmail();

    private async Task SendMailAsync()
    {
        if (IsEmailValid(_email) is false)
        {
            Snackbar.Add("Please enter a valid email address.", Severity.Error);
            Logger.LogError("Invalid email address: {Email}", _email);
            return;
        }

        try
        {
            var attachments = new IAttachment[]
            {
                new CsvAttachment<City>
                {
                    Content = await CtxCitiesDb.Cities.ToListAsync(),
                    TransformHeader = city => $"{nameof(city.Name)}, {nameof(city.Population)}, {nameof(city.IsCapital)}, {nameof(city.FoundationDate)}",
                    Transform = city => $"{city.Name}, {city.Population}, {city.IsCapital}, {city.FoundationDate?.ToString("yyyy-MM-dd") ?? null}"
                }
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var token = cts.Token;

            await MailService.SendEmailAsync(_email, "Cities Data", "Please find the attached CSV file.", attachments, token);
            Snackbar.Add("Email sent successfully!", Severity.Success);
            _email = string.Empty;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error sending email");
            Snackbar.Add("Failed to send email. Please try again later.", Severity.Error);
        }
    }

    private static bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email && ValidEmail().IsMatch(email);
        }
        catch
        {
            return false;
        }
    }
}