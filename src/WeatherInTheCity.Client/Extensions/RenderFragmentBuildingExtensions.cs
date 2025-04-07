using System.Text;
using Microsoft.AspNetCore.Components;

namespace WeatherInTheCity.Client.Extensions;

public static class RenderFragmentBuildingExtensions
{
    public static string AddStyles(this string content, Dictionary<string, string> tagStyles)
    {
        var sb = new StringBuilder(content.Length);
        var i = 0;

        while (i < content.Length)
        {
            if (content[i] == '<')
            {
                var tagEnd = content.IndexOf('>', i);
                if (tagEnd > i)
                {
                    var tag = content.Substring(i, tagEnd - i + 1);
                    if (tagStyles.TryGetValue(tag, out var replacement))
                    {
                        sb.Append(replacement);
                        i = tagEnd + 1;
                        continue;
                    }
                }
            }
            sb.Append(content[i]);
            i++;
        }

        return sb.ToString();
    }
    
    public static RenderFragment AsRenderFragment(this string content)
    {
        return builder => builder.AddMarkupContent(0, content);
    }
    
    public static RenderFragment AsRenderFragment(this string[] contents)
    {
        return builder =>
        {
            foreach (var item in contents)
            {
                builder.AddMarkupContent(0, item);
            }
        };
    }
}