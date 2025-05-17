using MediatR;
using Todo.Core.Agents;
using Todo.Core.Agents.Build;
using Todo.Core.Extensions;
using Todo.Core.Users;

namespace Todo.Core.Services;

public class TodoService(IAgentProvider agentProvider) : ITodoService, IRequestHandler<UserRequest, UserResponse>
{   
    public async Task<UserResponse> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();

        var sendTaskRequest = AgentExtensions.CreateUserSendTaskRequest(notification.SessionId, notification.Message);
     
        var agentTaskManager = agentProvider.GetTaskManager(AgentNames.OrchestratorAgent);

        var response = await agentTaskManager.SendTask(sendTaskRequest);

        return new UserResponse { Task = response.Task };
    }
}

public interface ITodoService
{
} 