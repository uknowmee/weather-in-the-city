using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using WeatherInTheCity.Ai;
using WeatherInTheCity.Cities;
using WeatherInTheCity.Weather;

namespace WeatherInTheCity.Components.Pages;

public partial class Cities
{
    private List<City> _cities = [];
    private City? _editingCity;
    private string _searchString = "";

    private Func<City, bool> QuickFilter => city =>
        string.IsNullOrWhiteSpace(_searchString) ||
        city.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
        (_searchString.Contains("capi", StringComparison.OrdinalIgnoreCase) && city.IsCapital) ||
        (int.TryParse(_searchString, out var population) && city.Population == population) ||
        city.FoundationDate?.ToString(DateFormat).Contains(_searchString) is true;

    private const string DateFormat = "yyyy-MM-dd";

    protected override async Task OnInitializedAsync()
    {
        _cities = await CtxCitiesDb.Cities.ToListAsync();
    }

    private async Task AddNewCity()
    {
        if (_cities.Any(c => c.Name == ""))
        {
            Logger.LogWarning("City already exists. Please fill the name before adding a new one.");
            Snackbar.Add("Default City already exists. Please fill the name before adding a new one.", Severity.Warning);
            return;
        }

        var newCity = City.Default();

        try
        {
            await CtxCitiesDb.AddAsync(newCity);
            await CtxCitiesDb.SaveChangesAsync();
            _cities.Add(newCity);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error adding new city");
            Snackbar.Add("Unexpected error occurred while adding new city.", Severity.Error);
        }
    }

    private async Task DeleteCityAsync(City city)
    {
        try
        {
            CtxCitiesDb.Remove(city);
            await CtxCitiesDb.SaveChangesAsync();
            _cities.Remove(city);
            Logger.LogInformation("City {CityId} deleted successfully", city.CityId);
            Snackbar.Add("City deleted successfully", Severity.Success);
        }
        catch (DbUpdateConcurrencyException e)
        {
            Logger.LogError(e, "Concurrency error while deleting city {CityId}", city.CityId);
            Snackbar.Add("Concurrency error occurred. Data reloaded.", Severity.Error);
            _cities = await CtxCitiesDb.Cities.ToListAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error deleting city {CityId}", city.CityId);
            Snackbar.Add("Unexpected error occurred while deleting city.", Severity.Error);
        }
    }

    private void StartedEditingCity(City city)
    {
        _editingCity = city.CreateCopy();
        Logger.LogInformation("Started editing city {CityId}", city.CityId);
    }

    private void CanceledEditingCity(City city)
    {
        if (_editingCity is null)
        {
            return;
        }

        city.CopyValuesFrom(_editingCity);
        Logger.LogInformation("Canceled editing city {CityId}", city.CityId);
        _editingCity = null;
    }

    private async Task CommittedCityChanges(City city)
    {
        try
        {
            var validationContext = new ValidationContext(city);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(city, validationContext, results, true);
            var validationErrors = string.Join(", ", results.Select(r => r.ErrorMessage));

            if (!isValid)
            {
                Logger.LogWarning("City validation failed: {ValidationErrors}", validationErrors);
                Snackbar.Add($"City validation failed: {validationErrors}", Severity.Warning);
                CanceledEditingCity(city);
                return;
            }

            CtxCitiesDb.Update(city);
            await CtxCitiesDb.SaveChangesAsync();
            Logger.LogInformation("City changes committed successfully for {CityId}", city.CityId);
            Snackbar.Add("City changes saved successfully", Severity.Success);
        }
        catch (DbUpdateConcurrencyException e)
        {
            Logger.LogError(e, "Concurrency error while committing city changes {CityId}", city.CityId);
            Snackbar.Add("Concurrency error occurred. Data reloaded.", Severity.Error);
            _cities = await CtxCitiesDb.Cities.ToListAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error committing city changes {CityId}", city.CityId);
            Snackbar.Add("Unexpected error occurred while saving city.", Severity.Error);
        }
    }

    private async Task OnFetchExternalDataAsync(City city)
    {
        Logger.LogInformation("Fetching external data for city {CityId}", city.CityId);

        var previousDescription = city.Description;
        var previousCondition = city.CurrentCondition;

        city.Description.Clear();
        city.CurrentCondition = null;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var token = cts.Token;

        var descriptionTask = Task.Run(async () =>
        {
            try
            {
                var request = new GenerateDescriptionRequest(city.CityId, city.Name);
                await foreach (var word in AiService.GetCityDescriptionAsync(request).WithCancellation(token))
                {
                    city.Description.Add(word);
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to fetch description for {CityId}", city.CityId);
                city.Description = previousDescription;
            }
        }, token);

        var weatherTask = Task.Run(async () =>
        {
            try
            {
                var request = new GenerateWeatherRequest(city.CityId, city.Name);
                var weather = await WeatherService.GetWeatherAsync(request, token);
                city.CurrentCondition = weather;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to fetch weather for {CityId}", city.CityId);
                city.CurrentCondition = previousCondition;
            }
        }, token);

        await Task.WhenAll(descriptionTask, weatherTask);

        Logger.LogInformation("Fetched external data for city {CityId}", city.CityId);
    }
}