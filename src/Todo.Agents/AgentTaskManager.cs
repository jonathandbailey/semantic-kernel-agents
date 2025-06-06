using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Todo.Core.A2A;
using Todo.Infrastructure.Azure;

namespace Todo.Agents;

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
            var agentState = agentStateStore.Update(agent.Name, agentTask.SessionId, agentTask.TaskId, agentTask);
        
            var textPart = request.Parameters.Message.Parts.First();

            agentState.ChatCompletionRequest = new ChatCompletionRequest
                { Message = textPart.Text, SessionId = agentTask.SessionId, TaskId = agentTask.TaskId };

            var response = await agent.InvokeAsync(agentState);

            await agentTaskRepository.SaveAsync(response.AgentTask);

            return new SendTaskResponse { Task = response.AgentTask };
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{agent.Name} Task Manager : {e.Message}");
            
            agentTask.SetInputRequiredFailed("We are not able to process your request at this time!");

            return new SendTaskResponse {  Task = agentTask };
        }
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