using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Todo.Api.Settings;
using Todo.Application.Dto;
using Todo.Core.Users;

namespace Todo.Api.Hubs
{
    public class UserMessageSender(IHubContext<UserHub> userHub, IUserConnectionManager userConnectionManager, IOptions<HubSettings> hubSettings) : IUserMessageSender
    {
        private Guid _sessionId = Guid.Empty;
        private Guid _userId = Guid.Empty;
        
        public async Task RespondAsync<T>(T payload, Guid userId)
        {
            var connections = userConnectionManager.GetConnections(userId);
            
            foreach (var connectionId in connections)
            {
                await userHub.Clients.Client(connectionId).SendAsync(hubSettings.Value.PromptChannel, payload);
            }
        }

        public void Initialize(Guid sessionId, Guid userId)
        {
            _sessionId = sessionId;
            _userId = userId;
        }

        public async Task StreamingMessageCallback(string content, bool isEndOfStream, Guid id)
        {
            var payLoad = new UserResponseDto {Id = id, Message = content, SessionId = _sessionId, IsEndOfStream = isEndOfStream };

            await RespondAsync(payLoad, _userId);
        }
    }
}
