﻿using Microsoft.EntityFrameworkCore;
using WeatherInTheCity.Framework;

namespace WeatherInTheCity.Cities;

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

    public virtual DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(DefaultCities.Values);
    }
}