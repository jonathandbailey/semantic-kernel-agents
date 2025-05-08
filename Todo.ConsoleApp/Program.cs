using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.ConsoleApp;
using Todo.ConsoleApp.Commands;
using Todo.ConsoleApp.Settings;
using Todo.ConsoleApp.Users;
using Todo.Core.Agents;
using Todo.Core.Extensions;
using Todo.Core.Settings;
using Todo.Core.Users;


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

    services.AddCoreServices(context.Configuration);

    services.AddSingleton<IUser, User>();
       
    services.AddSingleton<TodoApplication>();
    services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TodoApplication).Assembly));
});

builder.ConfigureLogging(logging =>
{
    logging.AddApplicationInsights();
});

try
{
    var host = builder.Build();

    using var cancellationTokenSource = new CancellationTokenSource();

    await host.Services.GetRequiredService<IAgentConfigurationProvider>().Load();

    await host.Services.GetRequiredService<TodoApplication>().RunAsync(cancellationTokenSource);

}
catch (Exception exception)
{
    Console.WriteLine($"Failed to Start the Application : {exception.Message}");
}