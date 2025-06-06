using MediatR;
using Todo.Agents;
using Todo.Agents.Build;
using Todo.Application.Dto;
using Todo.Application.Interfaces;
using Todo.Application.Users;
using Todo.Infrastructure;

namespace Todo.Application.Services;

public class TodoService(IAgentProvider agentProvider, IUserRepository userRepository, IUserMessageSender userMessageSender) : ITodoService, IRequestHandler<UserRequest, UserResponseDto>
{   
    public async Task<UserResponseDto> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();
      
        var agentTaskManager = agentProvider.GetTaskManager(AgentNames.OrchestratorAgent);

        var user = await userRepository.Get(notification.UserId);

        var response = await agentTaskManager.SendTask(notification.SendTaskRequest);

        var payLoad = new UserResponseDto { Message = response.Task.ExtractTextBasedOnResponse(), SessionId = response.Task.SessionId, TaskId = response.Task.TaskId };

        await userMessageSender.RespondAsync(payLoad, user.Id);

        return payLoad;
    }
}

public interface ITodoService
{
} 