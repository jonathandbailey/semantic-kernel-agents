using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Todo.Core.Settings;

namespace Todo.Core.Models;

public static class AppOpenTelemetry
{
    private static TracerProvider? _traceProvider;
    private static ILoggerFactory? _loggerFactory;
    
    public static ILoggerFactory CreateLoggerFactory(string applicationInsightsConnectionString, OpenTelemetrySettings settings)
    {
        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(settings.ApplicationName);

        AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

        var traceBuilder = Sdk.CreateTracerProviderBuilder();

        traceBuilder.SetResourceBuilder(resourceBuilder);

        foreach (var sourceName in settings.TraceProviderSourceNames)
        {
            traceBuilder.AddSource(sourceName);
        }

        traceBuilder.AddAzureMonitorTraceExporter(options =>
            options.ConnectionString = applicationInsightsConnectionString);
            
        _traceProvider = traceBuilder.Build();

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder);
                options.AddAzureMonitorLogExporter(exporterOptions => exporterOptions.ConnectionString = applicationInsightsConnectionString);
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });

        return _loggerFactory;
    }

    public static void Dispose()
    {
        _traceProvider?.Dispose();
        _loggerFactory?.Dispose();
    }
}