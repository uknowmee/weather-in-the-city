namespace WeatherInTheCity.Cities;

public interface IResetCitiesDbService
{
    Task<IResult> ResetDatabaseAsync(CancellationToken cancellationToken);
}

public class ResetCitiesDbService : IResetCitiesDbService
{
    private readonly CtxCitiesDb _ctxCitiesDb;
    private readonly ILogger<ResetCitiesDbService> _logger;

    public ResetCitiesDbService(CtxCitiesDb ctxCitiesDb, ILoggerFactory loggerFactory)
    {
        _ctxCitiesDb = ctxCitiesDb;
        _logger = loggerFactory.CreateLogger<ResetCitiesDbService>();
    }

    public async Task<IResult> ResetDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var token = cts.Token;

            await _ctxCitiesDb.Database.EnsureDeletedAsync(token);
            await _ctxCitiesDb.Database.EnsureCreatedAsync(token);

            _logger.LogInformation("Database reset successfully");
            return Results.Ok("Database reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting database");
            return Results.InternalServerError("Error resetting database");
        }
    }
}