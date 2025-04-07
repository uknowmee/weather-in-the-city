using Microsoft.EntityFrameworkCore;
using WeatherInTheCity.Framework;

namespace WeatherInTheCity.CitiesDb;

public class CitiesDbOptions : IDatabaseOptions
{
    public DatabaseType Type { get; set; } = DatabaseType.Postgres;
    public string ConnectionString { get; set; } = string.Empty;
}

public class CtxCitiesDb : DbContext
{
    public CtxCitiesDb(DbContextOptions<CtxCitiesDb> options) : base(options)
    {
    }
}