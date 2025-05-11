using MediatR;
using Todo.Core.Agents;
using Todo.Core.Communication;
using Todo.Core.Messaging;

namespace Todo.Core.Services;

public class TodoService(IMessagePublisher publisher, IAgentProvider agentProvider) : ITodoService, INotificationHandler<UserMessage>
{
    public async Task Handle(UserMessage notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();

        var agent = agentProvider.Get();
        
        var agentTask = new AgentTask();

        agentTask.History.Add(new AgentMessage {Message = notification.Message});
    
        var response = await agent.InvokeAsync(new ChatCompletionRequest {Message = notification.Message});

        await publisher.Publish(new AssistantMessage(response.Message));
    }
}

public interface ITodoService
{
} 