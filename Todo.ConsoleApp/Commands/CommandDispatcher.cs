using MediatR;
using Todo.ConsoleApp.Settings;
using Todo.Core.Messaging;

namespace Todo.ConsoleApp.Commands;

public class CommandDispatcher(IMediator mediator) : ICommandDispatcher
{
    private readonly Dictionary<string, Func<string, Task>> _commands = new();

    public void Initialize(CancellationTokenSource cancellationTokenSource)
    {
        _commands[Constants.ExitCommandKey] = async _ => await cancellationTokenSource.CancelAsync();
        _commands[Constants.ChatCommandKey] = async input => await mediator.Publish(new UserMessage { Message = input});
    }
    
    public async Task ExecuteCommandAsync(string input)
    {
        if (_commands.TryGetValue(input, out var command))
        {
            await command(input);
            return;
        }

        await _commands[Constants.ChatCommandKey](input);
    }
}

public interface ICommandDispatcher
{
    void Initialize(CancellationTokenSource cancellationTokenSource);
    Task ExecuteCommandAsync(string input);
}