using WeatherInTheCity.Cms;

namespace WeatherInTheCity.Components.Pages;

public partial class OtherProjects
{
    private readonly Dictionary<string, string> _tagStyles = new()
    {
        { "<p>", """<p class="mud-typography mud-typography-body2">""" }
    };

    private IEnumerable<Project>? _projects = [];

    protected override async Task OnInitializedAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            _projects = await CmsService.GetProjectsAsync(cts.Token);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error loading Projects");
            _projects = null;
        }
    }
}