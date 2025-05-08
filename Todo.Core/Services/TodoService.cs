using MediatR;
using Todo.Core.Agents;
using Todo.Core.Messaging;

namespace Todo.Core.Services;

public class TodoService(IAgentProvider agentProvider, IAgentConfigurationProvider agentConfigurationProvider) : ITodoService, INotificationHandler<UserMessage>
{
    public async Task Handle(UserMessage notification, CancellationToken cancellationToken)
    {
        await agentConfigurationProvider.Load();

        var agent = agentProvider.Get();

        await agent.Chat(notification.Message);
    }
}

public interface ITodoService
{
}