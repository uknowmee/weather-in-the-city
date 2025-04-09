using System.Text;

namespace WeatherInTheCity.Mail;

public interface IAttachment
{
    public string Filename { get; }
    public string MimeType { get; }
    public string StringContent { get; }
}

public class CsvAttachment<T> : IAttachment
{
    public string Filename => "attachment.csv";
    public string MimeType => "text/csv";
    public string StringContent => GenerateContent();
    
    public required Func<T, string>? TransformHeader { get; set; }
    public required Func<T, string> Transform { get; set; }
    public List<T> Content { get; set; } = [];
    
    private string GenerateContent()
    {
        var sb = new StringBuilder();

        if(Content.Count == 0)
        {
            return string.Empty;
        }
        
        sb.AppendLine(TransformHeader?.Invoke(Content[0]) ?? string.Empty);
        
        foreach (var item in Content)
        {
            sb.AppendLine(Transform(item));
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}