using MediatR;
using Todo.Core.Agents;
using Todo.Core.Communication;

namespace Todo.Core.Services;

public class TodoService(IAgentProvider agentProvider) : ITodoService, IRequestHandler<SendTaskRequest, SendTaskResponse>
{   
    public async Task<SendTaskResponse> Handle(SendTaskRequest notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();
        
        var agentTaskManager = agentProvider.GetTaskManager(AgentNames.OrchestratorAgent);

        var response = await agentTaskManager.SendTask(notification);

        return response;
    }
}

public interface ITodoService
{
} 