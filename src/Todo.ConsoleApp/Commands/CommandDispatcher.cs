using Todo.ConsoleApp.Settings;
using Todo.Core.Extensions;
using Todo.Core.Users;

namespace Todo.ConsoleApp.Commands;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IChatClient _httpChatClient;
    private readonly Dictionary<string, Func<string, Task>> _commands = new();
    private string _sessionId = string.Empty;
    private string _taskId = string.Empty;

    public CommandDispatcher(IChatClient httpChatClient)
    {
        _httpChatClient = httpChatClient;
    }

    public void Initialize(CancellationTokenSource cancellationTokenSource)
    {
        _commands[Constants.ExitCommandKey] = async _ => await cancellationTokenSource.CancelAsync();
        _commands[Constants.ChatCommandKey] = async input => await ChatViaApi(input);
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
   
    private async Task ChatViaApi(string input)
    {
        var sendTaskRequest = AgentExtensions.CreateSendTaskRequest(_taskId, _sessionId, input);
        var userRequest = new UserRequest { Message = input, SendTaskRequest = sendTaskRequest };
        
        var userResponse = await _httpChatClient.Send(userRequest);
        
        _sessionId = userResponse.Task.SessionId;
        _taskId = userResponse.Task.TaskId;
    }
}

public interface ICommandDispatcher
{
    void Initialize(CancellationTokenSource cancellationTokenSource);
    Task ExecuteCommandAsync(string input);
}