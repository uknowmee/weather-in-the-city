using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WeatherInTheCity.Mail;

public interface IAttachment
{
    public string Filename { get; }
    public string MimeType { get; }
    public string StringContent { get; }
}

public interface IMailService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage, IAttachment[]? attachments = null);
}

public class MailService : IMailService
{
    private readonly ILogger<MailService> _logger;
    private readonly SendGridOptions _sendGridOptions;

    public MailService(ILoggerFactory loggerFactory, IOptions<SendGridOptions> options)
    {
        _logger = loggerFactory.CreateLogger<MailService>();
        _sendGridOptions = options.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage, IAttachment[]? attachments = null)
    {
        var sendgridAttachments = attachments?
            .Select(attachment => new Attachment
            {
                Content = attachment.StringContent,
                Filename = attachment.Filename,
                Type = attachment.MimeType,
                Disposition = "attachment"
            })
            .ToList() ?? [];

        await ExecuteSending(
            subject,
            htmlMessage,
            email,
            sendgridAttachments
        );
    }

    private async Task ExecuteSending(string subject, string message, string toEmail, List<Attachment> attachments)
    {
        var client = new SendGridClient(_sendGridOptions.Key);
        var msg = new SendGridMessage
        {
            From = new EmailAddress(_sendGridOptions.EmailFrom, _sendGridOptions.Team),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message,
            Attachments = attachments
        };
        msg.AddTo(new EmailAddress(toEmail));

        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email to {Email} queued successfully!", toEmail);
        }
        else
        {
            _logger.LogError("Failure Email to {Email}", toEmail);
        }
    }
}