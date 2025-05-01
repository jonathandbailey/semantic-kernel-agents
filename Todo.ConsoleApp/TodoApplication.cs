using Todo.ConsoleApp.Settings;

namespace Todo.ConsoleApp;

public class TodoApplication
{
    private readonly Dictionary<string, Func<string, Task>> _commands = new();

    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
        _commands.Add(Constants.ExitCommandKey, async _ => await cancellationTokenSource.CancelAsync());

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Console.Write(Constants.UserCaret);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;

            if (_commands.TryGetValue(input, out var command))
            {
                await command(input);
            }
        }
    }
}