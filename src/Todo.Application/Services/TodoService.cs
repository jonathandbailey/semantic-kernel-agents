using MediatR;
using Todo.Application.Dto;
using Todo.Application.Users;
using Todo.Core.Users;
using Todo.Core.Vacations;
using Todo.Infrastructure;

namespace Todo.Application.Services;

public class TodoService(IUserRepository userRepository, IUserMessageSender userMessageSender, IVacationPlanService vacationPlanService, IOrchestrationService orchestrationService) : ITodoService, IRequestHandler<UserRequest, UserResponseDto>
{   
    public async Task<UserResponseDto> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        return await Execute(notification);
    }

    private async Task<UserResponseDto> Execute(UserRequest notification)
    {
        var vacationPlan = await vacationPlanService.GetAsync(notification.VacationPlanId);
        
        var sessionId = notification.SessionId == Guid.Empty ? Guid.NewGuid() : notification.SessionId;

        var user = await userRepository.Get(notification.UserId);

        userMessageSender.Initialize(sessionId, user.Id);

        var arguments = CreateArguments(vacationPlan);

        var responseState = await orchestrationService.InvokeAsync(sessionId, notification.Message, arguments, notification.Source, vacationPlan.Id, notification.Id);
      
        var payLoad = new UserResponseDto
        {
            Message = responseState.AgentState.Response.Content!, 
            SessionId = responseState.AgentState.SessionId, 
            VacationPlanId = responseState.VacationPlanId,
            Source = responseState.Source
        };
    
        return payLoad;
    }

    private Dictionary<string, string> CreateArguments(VacationPlan vacationPlan)
    {
        var arguments = new Dictionary<string, string> { { "VacationPlanId", vacationPlan.Id.ToString() } };

        foreach (var stage in vacationPlan.Stages)
        {
            arguments.Add(stage.Stage.ToString(), stage.Id.ToString());
        }

        return arguments;
    }
}



public interface ITodoService
{
} 