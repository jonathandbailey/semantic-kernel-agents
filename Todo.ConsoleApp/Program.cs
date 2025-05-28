using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.ConsoleApp;
using Todo.ConsoleApp.Commands;
using Todo.ConsoleApp.Settings;
using Todo.Core.Models;
using Todo.Core.Settings;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFiles(hostingContext.HostingEnvironment);
    config.AddEnvironmentVariables();
    config.AddUserSecrets<Program>();
});

builder.ConfigureServices((context, services) =>
{
    services.AddApplicationInsightsTelemetryWorkerService(options =>
    {
        options.ConnectionString = context.GetRequiredValue(SettingsConstants.ApplicationInsights);
    });

    services.AddHttpClient();

    services.Configure<ChatClientSetting>(context.Configuration.GetSection("ChatClientSetting"));

    services.AddSingleton<IHttpChatClient, HttpChatClient>();
    services.AddSingleton<TodoApplication>();
    services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
});

builder.ConfigureLogging(logging =>
{
    logging.ClearProviders(); 
});

try
{
    var host = builder.Build();

    using var cancellationTokenSource = new CancellationTokenSource();

    await host.Services.GetRequiredService<TodoApplication>().RunAsync(cancellationTokenSource);

}
catch (Exception exception)
{
    Console.WriteLine($"Failed to Start the Application : {exception.Message}");
}
finally
{
    AppOpenTelemetry.Dispose();
}