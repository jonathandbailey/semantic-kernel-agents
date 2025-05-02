using Todo.ConsoleApp.Settings;
using Todo.Core.Services;

namespace Todo.ConsoleApp;

public class TodoApplication(ITodoService todoService)
{
    private readonly Dictionary<string, Func<string, Task>> _commands = new()
    {
        { Constants.ExitCommandKey, async _ => await Task.CompletedTask }
    };

    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
        _commands[Constants.ExitCommandKey] = async _ => await cancellationTokenSource.CancelAsync();
        _commands[Constants.ChatCommandKey] = async input => await todoService.Chat(input);

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Console.Write(Constants.UserCaret);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;

            if (_commands.TryGetValue(input, out var command))
            {
                await command(input);
                continue;
            }

            await _commands[Constants.ChatCommandKey](input);
        }
    }
}