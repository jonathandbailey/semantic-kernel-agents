using MediatR;
using Todo.Core.Agents;
using Todo.Core.Agents.Build;
using Todo.Core.Users;

namespace Todo.Core.Services;

public class TodoService(IAgentProvider agentProvider) : ITodoService, IRequestHandler<UserRequest, UserResponse>
{   
    public async Task<UserResponse> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();
      
        var agentTaskManager = agentProvider.GetTaskManager(AgentNames.OrchestratorAgent);

        var response = await agentTaskManager.SendTask(notification.SendTaskRequest);

        return new UserResponse { Task = response.Task };
    }
}

public interface ITodoService
{
} 