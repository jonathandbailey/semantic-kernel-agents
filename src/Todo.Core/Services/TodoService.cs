using MediatR;
using Todo.Application.Agents;
using Todo.Application.Agents.A2A;
using Todo.Application.Agents.Build;
using Todo.Application.Infrastructure;
using Todo.Application.Interfaces;
using Todo.Application.Users;

namespace Todo.Application.Services;

public class TodoService(IAgentProvider agentProvider, IUserRepository userRepository, IUserMessageSender userMessageSender) : ITodoService, IRequestHandler<UserRequest, UserResponse>
{   
    public async Task<UserResponse> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();
      
        var agentTaskManager = agentProvider.GetTaskManager(AgentNames.OrchestratorAgent);

        var user = await userRepository.Get(notification.UserId);

        var response = await agentTaskManager.SendTask(notification.SendTaskRequest);

        var payLoad = new UserResponse { Task = response.Task };

        await userMessageSender.RespondAsync(payLoad, user.Id);

        return payLoad;
    }
    
    public async Task<AgentTask> Handle(SendTaskRequest sendTaskRequest)
    {
        await agentProvider.Build();

        var agentTaskManager = agentProvider.GetTaskManager(AgentNames.OrchestratorAgent);

        var response = await agentTaskManager.SendTask(sendTaskRequest);

        return response.Task;
    }
}

public interface ITodoService
{
} 