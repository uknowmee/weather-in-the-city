using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WeatherInTheCity.Weather;

namespace WeatherInTheCity.CitiesDb;

public class City
{
    public Guid CityId { get; set; } = Guid.NewGuid();
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    public bool IsCapital { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Population must be greater than or equal to 0.")] public int Population { get; set; }
    public DateOnly? FoundationDate { get; set; }

    [NotMapped] public CurrentCondition? CurrentCondition { get; set; }
    [NotMapped] public List<string> Description { get; set; } = [];
    [NotMapped] public DateTime? FoundationDateTime
    {
        get => FoundationDate?.ToDateTime(new TimeOnly(0, 0));
        set => FoundationDate = value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
    }

    [Obsolete("Only for EF", true)]
    public City()
    {
    }

    private City(string name, bool isCapital, int population, DateOnly? foundationDate = null)
    {
        Name = name;
        IsCapital = isCapital;
        Population = population;
        FoundationDate = foundationDate;
    }

    public City CreateCopy()
    {
        return new City(Name, IsCapital, Population, FoundationDate)
        {
            CityId = CityId,
            CurrentCondition = CurrentCondition,
            Description = Description
        };
    }

    public void CopyValuesFrom(City city)
    {
        CityId = city.CityId;
        Name = city.Name;
        IsCapital = city.IsCapital;
        Population = city.Population;
        FoundationDate = city.FoundationDate;
        CurrentCondition = city.CurrentCondition;
        Description = city.Description;
    }

    public static City Default()
    {
        return new City("", false, 0);
    }
}