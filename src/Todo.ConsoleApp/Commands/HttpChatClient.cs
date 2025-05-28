using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Todo.Core.Users;
using Todo.ConsoleApp.Settings;

namespace Todo.ConsoleApp.Commands
{
    public class HttpChatClient : IHttpChatClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ChatClientSetting _settings;

        public HttpChatClient(IHttpClientFactory httpClientFactory, IOptions<ChatClientSetting> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }

        public async Task<UserResponse> Send(UserRequest userRequest)
        {
            var client = _httpClientFactory.CreateClient();
            var url = _settings.BaseUrl.TrimEnd('/') + "/" + _settings.SendUrl.TrimStart('/');
            
            var response = await client.PostAsJsonAsync(url, userRequest);
            response.EnsureSuccessStatusCode();
            var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
            
            if (userResponse == null)
            {
                throw new InvalidOperationException("Unable to Deserialize Response.");
            }
            
            return userResponse;
        }
    }

    public interface IHttpChatClient
    {
        Task<UserResponse> Send(UserRequest userRequest);
    }
}
