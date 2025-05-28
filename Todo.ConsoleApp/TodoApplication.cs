using System.Diagnostics;
using Todo.ConsoleApp.Commands;
using Todo.ConsoleApp.Settings;


namespace Todo.ConsoleApp;

public class TodoApplication(ICommandDispatcher commandManager, IHubConnectionClient hubConnectionClient) 
{
    private readonly ActivitySource _trace = new($"Todo.ConsoleApp");

    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
        using var activity = _trace.StartActivity($"Application.{nameof(TodoApplication)}");

        commandManager.Initialize(cancellationTokenSource);

        await hubConnectionClient.InitializeStreamingConnectionAsync();

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Console.Write(Constants.UserCaret);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;

            await commandManager.ExecuteCommandAsync(input);
        }
    }
}