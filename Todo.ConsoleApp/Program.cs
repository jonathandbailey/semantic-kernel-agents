using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.ConsoleApp;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.Configure(hostingContext.HostingEnvironment);
});

builder.ConfigureServices((context, services) =>
{
    services.AddApplicationInsightsTelemetryWorkerService(options =>
    {
        options.ConnectionString = context.GetConfigurationValue(Constants.ApplicationInsights);
    });

    services.AddSingleton<TodoApplication>();
});

builder.ConfigureLogging(logging =>
{
    logging.AddApplicationInsights();
});

var host = builder.Build();

using var cancellationTokenSource = new CancellationTokenSource();

await host.Services.GetRequiredService<TodoApplication>().RunAsync(cancellationTokenSource);