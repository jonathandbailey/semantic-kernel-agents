using MediatR;
using Todo.Agents;
using Todo.Application.Dto;
using Todo.Application.Users;
using Todo.Infrastructure;

namespace Todo.Application.Services;

public class TodoService(IUserRepository userRepository, IVacationPlanService vacationPlanService, IOrchestrationService orchestrationService) : ITodoService, IRequestHandler<UserRequest, UserResponseDto>
{   
    public async Task<UserResponseDto> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        return await Execute(notification);
    }

    private async Task<UserResponseDto> Execute(UserRequest notification)
    {
        var vacationPlan = await vacationPlanService.GetAsync(notification.VacationPlanId);
        
        var user = await userRepository.Get(notification.UserId);

        var responseState = await orchestrationService.InvokeAsync(notification.SessionId!, notification.Message, user.Id);
      
        var payLoad = new UserResponseDto { Message = responseState.Responses.First().Content!, SessionId = responseState.GetSessionId(), TaskId = responseState.GetTaskId(), VacationPlanId = vacationPlan.Id };

        var agentTask = responseState.GetAgentTask();

        await vacationPlanService.UpdateAsync(vacationPlan, responseState.AgentName, agentTask.Status.State);
    
        return payLoad;
    }
}

public interface ITodoService
{
} 