using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todo.ConsoleApp;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((_, services) =>
{
    services.AddSingleton<TodoApplication>();
});

var host = builder.Build();

using var cancellationTokenSource = new CancellationTokenSource();

await host.Services.GetRequiredService<TodoApplication>().RunAsync(cancellationTokenSource);