using Todo.ConsoleApp.Commands;
using Todo.ConsoleApp.Settings;

namespace Todo.ConsoleApp;

public class TodoApplication(ICommandDispatcher commandManager)
{
    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
        commandManager.Initialize(cancellationTokenSource);

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Console.Write(Constants.UserCaret);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;

            await commandManager.ExecuteCommandAsync(input);
        }
    }
}