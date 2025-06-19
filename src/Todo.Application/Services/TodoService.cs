using System.Text;
using MediatR;
using Todo.Agents;
using Todo.Application.Dto;
using Todo.Application.Interfaces;
using Todo.Application.Users;
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
        
        var user = await userRepository.Get(notification.UserId);

        var arguments = CreateArguments(vacationPlan);

        var responseState = await orchestrationService.InvokeAsync(notification.SessionId!, notification.Message, arguments,
            async (content, sessionId, isEndOfStream) =>
            {
                var payLoad = new UserResponseDto { Message = content.Content!, SessionId = sessionId, IsEndOfStream = isEndOfStream };

                await userMessageSender.RespondAsync(payLoad, user.Id);
            });
      
        var payLoad = new UserResponseDto { Message = responseState.Responses.First().Content!, SessionId = responseState.GetSessionId(), VacationPlanId = vacationPlan.Id };

        var agentTask = responseState.GetAgentTask();

        await vacationPlanService.UpdateAsync(vacationPlan, responseState.AgentName, agentTask.Status.State);
    
        return payLoad;
    }

    private Dictionary<string, string> CreateArguments(VacationPlan vacationPlan)
    {
        var stringBuilder = new StringBuilder();

        foreach (var vacationPlanStage in vacationPlan.Stages)
        {
            stringBuilder.AppendLine($"{vacationPlanStage.Stage}:{vacationPlanStage.Status}");
        }

        return new Dictionary<string, string> { {"VacationPlanStatus", stringBuilder.ToString()}};
    }
}



public interface ITodoService
{
} 