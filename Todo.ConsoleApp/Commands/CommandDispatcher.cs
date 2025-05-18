using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Todo.ConsoleApp.Settings;
using Todo.Core.Extensions;
using Todo.Core.Users;

namespace Todo.ConsoleApp.Commands;

public class CommandDispatcher(IServiceScopeFactory scopeFactory) : ICommandDispatcher
{
    private readonly Dictionary<string, Func<string, Task>> _commands = new();

    private string _sessionId = string.Empty;
    private string _taskId = string.Empty;

    public void Initialize(CancellationTokenSource cancellationTokenSource)
    {
        _commands[Constants.ExitCommandKey] = async _ => await cancellationTokenSource.CancelAsync();
        _commands[Constants.ChatCommandKey] = async input => await Chat(input);
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

    private async Task Chat(string input)
    {
        using var scope = scopeFactory.CreateScope();

        var publisher = scope.ServiceProvider.GetRequiredService<IMediator>();

        var sendTaskRequest = AgentExtensions.CreateUserSendTaskRequest(_taskId, _sessionId, input);

        var response = await publisher.Send(new UserRequest { Message = input, SendTaskRequest = sendTaskRequest});

        Console.WriteLine($"{Constants.SystemCaret}{response.Task.ExtractTextBasedOnResponse()}");

        _sessionId = response.Task.SessionId;
        _taskId = response.Task.TaskId;
    }
}

public interface ICommandDispatcher
{
    void Initialize(CancellationTokenSource cancellationTokenSource);
    Task ExecuteCommandAsync(string input);
}