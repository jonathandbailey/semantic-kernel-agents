using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using Todo.Agents;
using Todo.Agents.Build;
using Todo.Agents.Communication;
using Todo.Application.Dto;
using Todo.Application.Interfaces;
using Todo.Application.Users;
using Todo.Core.A2A;
using Todo.Core.Vacations;
using Todo.Infrastructure;
using Todo.Infrastructure.File;

namespace Todo.Application.Services;

public class TodoService(IAgentProvider agentProvider, IUserRepository userRepository, IUserMessageSender userMessageSender, IVacationPlanRepository vacationPlanRepository) : ITodoService, IRequestHandler<UserRequest, UserResponseDto>
{   
    public async Task<UserResponseDto> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        return await Execute(notification);
    }

    private async Task<UserResponseDto> Execute(UserRequest notification)
    {
        var vacationPlan = await CreateOrLoadVacationPlan(notification.VacationPlanId);
        
        var user = await userRepository.Get(notification.UserId);

        var responseState = await CreateAndCallOrchestrator(notification);

        var agentAction = GetAgentResponse(responseState);

        var workerAgent = await agentProvider.Create(agentAction.AgentName, async (content, isEndOfStream) =>
        {
            var payLoad = new UserResponseDto { Message = content.Content!, SessionId = responseState.GetSessionId(), TaskId = responseState.GetTaskId(), IsEndOfStream = isEndOfStream};

            await userMessageSender.RespondAsync(payLoad, user.Id);
            
        });

        var agentWorkerState = CreateWorkerState(responseState, agentAction.Message);

        var workerResponseState = await workerAgent.InvokeAsync(agentWorkerState);
     
        var payLoad = new UserResponseDto { Message = workerResponseState.Responses.First().Content!, SessionId = agentWorkerState.GetSessionId(), TaskId = agentWorkerState.GetTaskId(), VacationPlanId = vacationPlan.Id };

        UpdateVacationPlanState(vacationPlan, agentWorkerState, agentAction);

        await vacationPlanRepository.Save(vacationPlan);

        return payLoad;
    }

    private void UpdateVacationPlanState(VacationPlan vacationPlan, AgentState agentState,
        AgentTaskRequest agentTaskRequest)
    {
        var agentTask = agentState.GetAgentTask();

        if (agentTask.Status.State == AgentTaskState.Completed)
        {
            if (agentTaskRequest.AgentName == "TravelAgent")
            {
                vacationPlan.UpdateStageStatus(PlanStage.Travel, PlanStatus.Completed);
            }
        }
    }

    private async Task<VacationPlan> CreateOrLoadVacationPlan(Guid id)
    {
        if (id == Guid.Empty)
        {
            var vacationPlan = new VacationPlan(Guid.NewGuid(), PlanStatus.Open, "New Vacation Plan", "No description available.");

            vacationPlan.AddStage(new TravelPlan(Guid.NewGuid(), "Travel Plan", "No description available", PlanStage.Travel, PlanStatus.Open));
            
            return vacationPlan;
        }

        return await vacationPlanRepository.Load(id);
    }

    private async Task<AgentState> CreateAndCallOrchestrator(UserRequest userRequest)
    {
        var orchestrator = await agentProvider.Create(AgentNames.OrchestratorAgent);

        var agentState = CreateOrchestrationState(userRequest);

        return await orchestrator.InvokeAsync(agentState);
    }

    private static AgentState CreateWorkerState(AgentState orchestrationState, string message)
    {
        var taskId = Guid.NewGuid().ToString();
        var sessionId = orchestrationState.GetSessionId();
        
        var state = new AgentState { Request = new ChatMessageContent(AuthorRole.User, message) };

        if (!string.IsNullOrWhiteSpace(taskId))
        {
            state.SetTaskId(taskId);
        }

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            state.SetSessionId(sessionId);
        }

        return state;
    }

    private static AgentState CreateOrchestrationState(UserRequest notification)
    {
        var state = new AgentState { Request = new ChatMessageContent(AuthorRole.User, notification.Message) };

        if (!string.IsNullOrWhiteSpace(notification.TaskId))
        {
            state.SetTaskId(notification.TaskId);
        }

        if (!string.IsNullOrWhiteSpace(notification.SessionId))
        {
            state.SetSessionId(notification.SessionId);
        }

        return state;
    }
   
    private static AgentTaskRequest GetAgentResponse(AgentState state)
    {
        var message = state.Responses.First().Content;

        var agentResponse = JsonSerializer.Deserialize<AgentTaskRequest>(message!);

        if (agentResponse == null)
        {
            throw new AgentException($"Failed to deserialize agent response: {message}");
        }

        return agentResponse;
    }
}

public interface ITodoService
{
} 