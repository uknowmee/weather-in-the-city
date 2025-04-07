using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;

namespace WeatherInTheCity.Framework;

public class SeqOptions
{
    public string AppName { get; set; } = "Unknown";
    public string Host { get; set; } = "http://localhost:5341";
    public string? ApiKey { get; set; }
}

public static partial class Extensions
{
    private static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog();
        return builder;
    }

    private static IHostApplicationBuilder AddLogger(this IHostApplicationBuilder builder)
    {
        var seqOptions = new SeqOptions();
        builder.Configuration.GetSection(nameof(SeqOptions)).Bind(seqOptions);

        var loggerConfiguration = new LoggerConfiguration();
        var controlLevelSwitch = new LoggingLevelSwitch();
        var consoleLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Debug);
        var fileLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
        loggerConfiguration
            .MinimumLevel.ControlledBy(controlLevelSwitch)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.Async(sinkCfg => sinkCfg.File(
                    path: "./logs/log-.jsonl",
                    formatter: new JsonFormatter(),
                    rollingInterval: RollingInterval.Day,
                    levelSwitch: fileLevelSwitch
                )
            )
            .WriteTo.Async(sinkCfg => sinkCfg.Console(
                    outputTemplate: "[{Timestamp:o} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    levelSwitch: consoleLevelSwitch
                )
            )
            .WriteTo.Seq(
                serverUrl: seqOptions.Host,
                apiKey: seqOptions.ApiKey,
                controlLevelSwitch: controlLevelSwitch
            )
            .Enrich.WithProperty("Application", seqOptions.AppName);

        Log.Logger = loggerConfiguration.CreateLogger();
        var factory = new LoggerFactory([new SerilogLoggerProvider(Log.Logger)]);
        builder.Services.AddSingleton(factory);

        return builder;
    }

    private static IApplicationBuilder UseContextLogger(this IApplicationBuilder app) => app.UseSerilogRequestLogging();
}