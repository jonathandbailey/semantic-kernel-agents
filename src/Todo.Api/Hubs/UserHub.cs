using Microsoft.AspNetCore.SignalR;
using Todo.Api.Extensions;

namespace Todo.Api.Hubs;

public class UserHub(IUserConnectionManager userConnectionManager) : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = GetUserId();

        if (userId.HasValue)
        {
            userConnectionManager.AddConnection(userId.Value, Context.ConnectionId);
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();

        if (userId.HasValue)
        {
            userConnectionManager.RemoveConnection(Context.ConnectionId);
        }

        return base.OnDisconnectedAsync(exception);
    }

    private Guid? GetUserId()
    {
       return Context.User?.Id();
    }
}