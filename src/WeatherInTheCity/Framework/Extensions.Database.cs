using Microsoft.EntityFrameworkCore;

namespace WeatherInTheCity.Framework;

public interface IDatabaseOptions
{
    public DatabaseType Type { get; set; }
    public string ConnectionString { get; set; }
}

public enum DatabaseType
{
    Postgres = 0
}

public static partial class Extensions
{
    public static IHostApplicationBuilder AddDatabase<TDatabaseCtx, TDatabaseOptions>(this IHostApplicationBuilder builder)
        where TDatabaseCtx : DbContext
        where TDatabaseOptions : IDatabaseOptions, new()
    {
        var dbOptions = new TDatabaseOptions();
        builder.Configuration.GetRequiredSection(typeof(TDatabaseOptions).Name).Bind(dbOptions);

        builder.Services.AddDbContext<TDatabaseCtx>(options =>
            {
                _ = dbOptions.Type switch
                {
                    DatabaseType.Postgres => options.UseNpgsql(dbOptions.ConnectionString),
                    _ => throw new InvalidOperationException($"Unsupported database type: {dbOptions.Type}")
                };
            }
        );

        return builder;
    }

    public static IApplicationBuilder CreateDatabase<TDatabase>(this IApplicationBuilder app) where TDatabase : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TDatabase>();
        dbContext.Database.EnsureCreated();

        return app;
    }
}