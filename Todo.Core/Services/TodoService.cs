using MediatR;
using Todo.Core.Agents;
using Todo.Core.Messaging;

namespace Todo.Core.Services;

public class TodoService(IMessagePublisher publisher, IAgentProvider agentProvider, IAgentConfigurationProvider agentConfigurationProvider) : ITodoService, INotificationHandler<UserMessage>
{
    public async Task Handle(UserMessage notification, CancellationToken cancellationToken)
    {
        await agentConfigurationProvider.Load();

        var agent = agentProvider.Get();

        var responses = await agent.InvokeAsync(notification.Message);

        foreach (var response in responses)
        {
            await publisher.Publish(new AssistantMessage(response));
        }
    }
}

public interface ITodoService
{
}