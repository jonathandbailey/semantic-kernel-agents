﻿using Todo.ConsoleApp.Dto;
using Todo.ConsoleApp.Settings;

namespace Todo.ConsoleApp.Services;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IChatClient _httpChatClient;
    private readonly Dictionary<string, Func<string, Task>> _commands = new();
    private Guid _sessionId = Guid.Empty;
    private Guid _vacationPlanId = Guid.Empty;
    private string _source = string.Empty;

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
        var userResponse = await _httpChatClient.Send(new UserRequestDto { Message = input, SessionId = _sessionId, VacationPlanId = _vacationPlanId, Source = _source});

        if (userResponse.HasError)
        {
            Console.WriteLine(userResponse.Message);
        }

        _sessionId = userResponse.SessionId;
        _vacationPlanId = userResponse.VacationPlanId;
        _source = userResponse.Source;
    }
}

public interface ICommandDispatcher
{
    void Initialize(CancellationTokenSource cancellationTokenSource);
    Task ExecuteCommandAsync(string input);
}