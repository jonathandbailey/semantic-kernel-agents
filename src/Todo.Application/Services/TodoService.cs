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
using Todo.Infrastructure;

namespace Todo.Application.Services;

public class TodoService(IAgentProvider agentProvider, IUserRepository userRepository, IUserMessageSender userMessageSender) : ITodoService, IRequestHandler<UserRequest, UserResponseDto>
{   
    public async Task<UserResponseDto> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        return await Execute(notification);
    }

    private async Task<UserResponseDto> Execute(UserRequest notification)
    {
        var responseState = await CreateAndCallOrchestrator(notification);

        var agentAction = GetAgentResponse(responseState);

        var workerAgent = await agentProvider.Create(agentAction.AgentName);

        var agentWorkerState = CreateWorkerState(responseState, agentAction.Message);

        var workerResponseState = await workerAgent.InvokeAsync(agentWorkerState);

        var workResponse = GetAgentWorkerResponse(workerResponseState);
      
        var user = await userRepository.Get(notification.UserId);
     
        var payLoad = new UserResponseDto { Message = workResponse.Message, SessionId = agentWorkerState.GetSessionId(), TaskId = agentWorkerState.GetTaskId() };

        await userMessageSender.RespondAsync(payLoad, user.Id);

        return payLoad;
    }

    private async Task<AgentState> CreateAndCallOrchestrator(UserRequest userRequest)
    {
        var orchestrator = await agentProvider.Create(AgentNames.OrchestratorAgent);

        var agentState = CreateOrchestrationState(userRequest);

        return await orchestrator.InvokeAsync(agentState);
    }

    private static AgentState CreateWorkerState(AgentState orchestrationState, string message)
    {
        var taskId = orchestrationState.GetTaskId();
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

    private static AgentActionResponse GetAgentWorkerResponse(AgentState state)
    {
        var message = state.Responses.First().Content;

        var agentResponse = JsonSerializer.Deserialize<AgentActionResponse>(message!);

        if (agentResponse == null)
        {
            throw new AgentException($"Failed to deserialize agent response: {message}");
        }

        return agentResponse;
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