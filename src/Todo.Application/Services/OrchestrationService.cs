using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using Todo.Agents;
using Todo.Agents.Build;
using Todo.Agents.Communication;
using Todo.Application.Dto;
using Todo.Application.Interfaces;
using Todo.Infrastructure;

namespace Todo.Application.Services;

public class OrchestrationService(IAgentProvider agentProvider, IUserMessageSender userMessageSender) : IOrchestrationService
{
    public async Task<AgentState> InvokeAsync(string sessionId, string message, Guid userId)
    {
        var orchestrator = await agentProvider.Create(AgentNames.OrchestratorAgent);

        var state = new AgentState(AgentNames.OrchestratorAgent) { Request = new ChatMessageContent(AuthorRole.User, message) };

        state.SetTaskId(Guid.NewGuid().ToString());

        state.SetSessionId(!string.IsNullOrWhiteSpace(sessionId) ? sessionId : Guid.NewGuid().ToString());

        state = await orchestrator.InvokeAsync(state);

        var agentAction = GetAgentResponse(state);

        var workerAgent = await agentProvider.Create(agentAction.AgentName, async (content, isEndOfStream) =>
        {
            var payLoad = new UserResponseDto { Message = content.Content!, SessionId = state.GetSessionId(), TaskId = state.GetTaskId(), IsEndOfStream = isEndOfStream };

            await userMessageSender.RespondAsync(payLoad, userId);

        });

        var agentWorkerState = CreateWorkerState(state, agentAction);

        var workerResponseState = await workerAgent.InvokeAsync(agentWorkerState);

        return workerResponseState;
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

    private static AgentState CreateWorkerState(AgentState orchestrationState, AgentTaskRequest agentActionResponse)
    {
        var taskId = Guid.NewGuid().ToString();
        var sessionId = orchestrationState.GetSessionId();

        var state = new AgentState(agentActionResponse.AgentName) { Request = new ChatMessageContent(AuthorRole.User, agentActionResponse.Message) };

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
}

public interface IOrchestrationService
{
    Task<AgentState> InvokeAsync(string sessionId, string message, Guid userId);
}