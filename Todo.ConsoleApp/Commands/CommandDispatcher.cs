using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Todo.ConsoleApp.Settings;
using Todo.Core.Communication;

namespace Todo.ConsoleApp.Commands;

public class CommandDispatcher(IServiceScopeFactory scopeFactory) : ICommandDispatcher
{
    private readonly Dictionary<string, Func<string, Task>> _commands = new();

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

        var response =   await publisher.Send(
            new SendTaskRequest 
                { Parameters  = new TaskSendParameters
                {
                    SessionId = Guid.NewGuid().ToString(),
                    Message = new Message
                    {
                        Parts = [new TextPart { Text = input }]
                    }
                }
            });

        Console.WriteLine(response.Task.Status.Message.Parts.First().Text);
    }
}

public interface ICommandDispatcher
{
    void Initialize(CancellationTokenSource cancellationTokenSource);
    Task ExecuteCommandAsync(string input);
}