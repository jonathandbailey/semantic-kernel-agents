using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Todo.ConsoleApp.Settings;
using Todo.Core.Extensions;
using Todo.Core.Users;

namespace Todo.ConsoleApp.Commands;

public class HubConnectionClient(IOptions<ChatClientSetting> settings) : IHubConnectionClient
{
    private HubConnection? _hubConnection;

    public async Task InitializeStreamingConnectionAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(settings.Value.BaseUrl + settings.Value.HubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<UserResponse>(settings.Value.PromptChannel, (message) =>
        {
            Console.WriteLine($"{Constants.SystemCaret}{message.Task.ExtractTextBasedOnResponse()}");
        });

        await _hubConnection.StartAsync();
    }
}

public interface IHubConnectionClient
{
    Task InitializeStreamingConnectionAsync();
}