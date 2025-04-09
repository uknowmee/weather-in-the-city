using WeatherInTheCity.Cms;

namespace WeatherInTheCity.Components.Pages;

public partial class Home
{
    private readonly Dictionary<string, string> _tagStyles = new()
    {
        { "<p>", """<p class="mud-typography mud-typography-body1 mb-4">""" },
        { "<h3>", """<h3 class="mud-typography mud-typography-h3">""" },
        { "<h6>", """<h6 class="mud-typography mud-typography-h6">""" },
        { "<strong>", """<strong class="mud-typography mud-typography-h5">""" }
    };

    private About? _about;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            _about = await CmsService.GetAppAboutAsync(cts.Token);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error loading About");
            const string failed = "<p>Failed to load content</p>";
            _about = new About(failed, new Content(failed), new Content(failed));
        }
    }
}