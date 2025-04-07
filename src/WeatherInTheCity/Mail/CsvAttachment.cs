using System.Text;

namespace WeatherInTheCity.Mail;

public class CsvAttachment<T> : IAttachment
{
    public string Filename => "attachment.csv";
    public string MimeType => "text/csv";
    public string StringContent => GenerateContent();
    
    public List<T> Content { get; set; } = [];

    private string GenerateContent()
    {
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();

        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        foreach (var item in Content)
        {
            sb.AppendLine(string.Join(",", properties.Select(p => p.GetValue(item)?.ToString()?.Replace(",", " ") ?? string.Empty)));
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}