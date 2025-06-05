using Microsoft.AspNetCore.SignalR;
using Todo.Application.Interfaces;

namespace Todo.Api.Hubs
{
    public class UserMessageSender(IHubContext<UserHub> userHub, IUserConnectionManager userConnectionManager) : IUserMessageSender
    {
        public async Task RespondAsync<T>(T payload, Guid userId)
        {
            var connections = userConnectionManager.GetConnections(userId);
            foreach (var connectionId in connections)
            {
                await userHub.Clients.Client(connectionId).SendAsync("ReceiveMessage", payload);
            }
        }
    }
}
