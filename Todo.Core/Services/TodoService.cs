using MediatR;
using Todo.Core.Agents;
using Todo.Core.Communication;
using Todo.Core.Messaging;
using Todo.Core.Middleware;

namespace Todo.Core.Services;

public class TodoService(IMessagePublisher publisher, IAgentProvider agentProvider) : ITodoService, INotificationHandler<UserMessage>
{
    public async Task Handle(UserMessage notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();

        var agent = agentProvider.Get();

        var agentBuild = new AgentMiddlewareBuilder();

        agentBuild.Use(new AgentMiddleware(agent));

        var agentMiddleware = agentBuild.Build();

        var agentTask = new AgentTask();

        agentTask.History.Add(new AgentMessage() {Message = notification.Message});
    
        var responses = await agentMiddleware(agentTask);

        foreach (var response in responses.Artifacts)
        {
            await publisher.Publish(new AssistantMessage(response.Message));
        }
    }
}

public interface ITodoService
{
}