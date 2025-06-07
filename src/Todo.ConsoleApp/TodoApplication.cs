using Todo.ConsoleApp.Services;
using Todo.ConsoleApp.Settings;

namespace Todo.ConsoleApp;

public class TodoApplication(ICommandDispatcher commandManager, IChatClient chatClient) 
{
    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
        commandManager.Initialize(cancellationTokenSource);

        await chatClient.InitializeStreamingConnectionAsync();

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Console.Write(Constants.UserCaret);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;

            await commandManager.ExecuteCommandAsync(input);
        }
    }
}