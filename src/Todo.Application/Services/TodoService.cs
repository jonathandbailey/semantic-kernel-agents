using MediatR;
using Todo.Application.Agents;
using Todo.Application.Agents.Build;
using Todo.Application.Interfaces;
using Todo.Application.Users;
using Todo.Infrastructure;

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
}

public interface ITodoService
{
} 