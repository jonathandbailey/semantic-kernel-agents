using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Todo.Api.Extensions;

namespace Todo.Api.Hubs;

public class UserHub(IUserConnectionManager userConnectionManager) : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.User?.Id();

        if (userId != null)
        {
            userConnectionManager.AddConnection(userId.Value, Context.ConnectionId);
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier);

        if (userId != null && !string.IsNullOrEmpty(userId.Value))
        {
            userConnectionManager.RemoveConnection(Context.ConnectionId);
        }

        return base.OnDisconnectedAsync(exception);
    }
}