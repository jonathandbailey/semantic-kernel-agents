using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;
using Todo.Core.Messaging;

namespace Todo.Core.Services;

public class TodoService([FromKeyedServices(AgentNames.TaskAgent)] IAgent agent) : ITodoService, INotificationHandler<UserMessage>
{
    public async Task Handle(UserMessage notification, CancellationToken cancellationToken)
    {
        await agent.Chat(notification.Message);
    }
}

public interface ITodoService
{
}