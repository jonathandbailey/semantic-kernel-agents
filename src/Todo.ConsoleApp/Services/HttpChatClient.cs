using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Todo.ConsoleApp.Dto;
using Todo.ConsoleApp.Settings;

namespace Todo.ConsoleApp.Services
{
    public class ChatClient : IChatClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ChatClientSetting _settings;
        private HubConnection? _hubConnection;
        private TaskCompletionSource? _taskCompletionSource;

        public ChatClient(IHttpClientFactory httpClientFactory, IOptions<ChatClientSetting> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }
        public async Task<UserResponseDto> Send(UserRequestDto userRequest)
        {
            _taskCompletionSource = new TaskCompletionSource();
            
            var url = $"{_settings.BaseUrl}{_settings.SendUrl}";

            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsJsonAsync(url, userRequest);

                response.EnsureSuccessStatusCode();

                var userResponse = await response.Content.ReadFromJsonAsync<UserResponseDto>();

                if (userResponse == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to deserialize Response. Task Id : {userRequest.TaskId}, Session Id : {userRequest.SessionId}, Url : {url}");
                }

                return userResponse;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new UserResponseDto { HasError = true, SessionId = userRequest.SessionId, TaskId = userRequest.TaskId};
            }
            finally
            {
                await _taskCompletionSource.Task;
            }
        }

        public async Task InitializeStreamingConnectionAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_settings.BaseUrl + _settings.HubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<UserResponseDto>(_settings.PromptChannel, (message) =>
            {
                Console.WriteLine($"{Constants.SystemCaret}{message.Message}");

                _taskCompletionSource?.SetResult();
            });

            await _hubConnection.StartAsync();
        }
    }

    public interface IChatClient
    {
        Task InitializeStreamingConnectionAsync();
        Task<UserResponseDto> Send(UserRequestDto userRequest);
    }
}
