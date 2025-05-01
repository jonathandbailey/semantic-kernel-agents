using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.ConsoleApp;
using Todo.ConsoleApp.Settings;
using Todo.Core;
using Todo.Core.Infrastructure;
using Todo.Core.Settings;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.Configure(hostingContext.HostingEnvironment);
});

builder.ConfigureServices((context, services) =>
{
    services.AddApplicationInsightsTelemetryWorkerService(options =>
    {
        options.ConnectionString = context.GetRequiredValue(Constants.ApplicationInsights);
    });

    services.Configure<AzureStorageSettings>(context.GetRequiredSection<AzureStorageSettings>());
    services.AddSingleton<IAgentTemplateRepository, AgentTemplateRepository>();
  
    services.AddSingleton(SemanticKernelBuilder.CreateKernel(context.GetRequiredSetting<LanguageModelSettings>()));

    services.AddSingleton<TodoApplication>();
});

builder.ConfigureLogging(logging =>
{
    logging.AddApplicationInsights();
});

var host = builder.Build();

using var cancellationTokenSource = new CancellationTokenSource();

await host.Services.GetRequiredService<TodoApplication>().RunAsync(cancellationTokenSource);