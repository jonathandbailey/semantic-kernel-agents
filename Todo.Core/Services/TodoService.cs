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
        
        var agentTaskManager = new AgentTaskManager(agent);

        var sendTaskRequest = new SendTaskRequest();

        sendTaskRequest.Parameters.Message.Parts.Add(new TextPart {Text = notification.Message});

        var response = await agentTaskManager.SendTask(sendTaskRequest);

        await publisher.Publish(new AssistantMessage(response.Message));
    }
}

public interface ITodoService
{
} 