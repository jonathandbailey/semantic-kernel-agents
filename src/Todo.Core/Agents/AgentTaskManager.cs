using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using Todo.Core.Agents.A2A;
using Todo.Core.Communication;
using Todo.Core.Extensions;
using Todo.Core.Infrastructure;

namespace Todo.Core.Agents;

public class AgentTaskManager(IAgent agent, ILogger<AgentTaskManager> logger, IAgentTaskRepository agentTaskRepository, IAgentStateStore agentStateStore) : IAgentTaskManager
{
    private readonly ActivitySource _trace = new($"Todo.Agent.TaskManager.{agent.Name}");

    public async Task<SendTaskResponse> SendTask(SendTaskRequest request)
    {
        using var activity = _trace.StartActivity($"TaskManager.{agent.Name}.{nameof(SendTask)}");
  
        var agentTask = await GetOrCreateAgentTask(request);

        activity?.SetTag("SessionId", agentTask.SessionId);
        activity?.SetTag("Task Id", agentTask.TaskId);

        try
        {
            agentStateStore.Update(agent.Name, agentTask.SessionId, agentTask.TaskId);
            
            var textPart = request.Parameters.Message.Parts.First();

            var response = await agent.InvokeAsync(new ChatCompletionRequest { Message = textPart.Text, SessionId = agentTask.SessionId, TaskId = agentTask.TaskId});

            var agentResponse = GetAgentResponse(response);

            agentTask.SetTaskState(agentResponse);

            await agentTaskRepository.SaveAsync(agentTask);

            return new SendTaskResponse { Task = agentTask };
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{agent.Name} Task Manager : {e.Message}");
            
            agentTask.SetInputRequiredFailed("We are not able to process your request at this time!");

            return new SendTaskResponse {  Task = agentTask };
        }
    }

    private AgentActionResponse GetAgentResponse(ChatCompletionResponse chatCompletionResponse)
    {
        var agentResponse = JsonSerializer.Deserialize<AgentActionResponse>(chatCompletionResponse.Message);

        if (agentResponse == null)
        {
            logger.LogError($"{agent.Name} Failed to deserialize agent response: {chatCompletionResponse.Message}");
            
            throw new AgentException($"{agent.Name} :Failed to deserialize agent response: {chatCompletionResponse.Message}");
        }

        return agentResponse;
    }
    
    private async Task<AgentTask> GetOrCreateAgentTask(SendTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Parameters.Id))
        {
            return request.CreateAgentTask();
        }

        if (await agentTaskRepository.Exists(request.Parameters.Id))
        {
            var agentTask = await agentTaskRepository.LoadAsync(request.Parameters.Id);

            if (!string.IsNullOrEmpty(request.Parameters.SessionId))
            {
                agentTask.SessionId = request.Parameters.SessionId;
            }

            return agentTask;
        }
        
        var newTask = request.CreateAgentTask();

        newTask.TaskId = request.Parameters.Id;

        if (!string.IsNullOrEmpty(request.Parameters.SessionId))
        {
            newTask.SessionId = request.Parameters.SessionId;
        }

        return newTask;
    }
}

public interface IAgentTaskManager
{
    Task<SendTaskResponse> SendTask(SendTaskRequest request);
}