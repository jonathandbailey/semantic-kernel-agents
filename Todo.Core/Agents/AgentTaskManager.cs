using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using Todo.Core.Agents.A2A;
using Todo.Core.Communication;
using Todo.Core.Extensions;

namespace Todo.Core.Agents;

public class AgentTaskManager(IAgent agent, ILogger<AgentTaskManager> logger) : IAgentTaskManager
{
    private readonly ActivitySource _trace = new($"Todo.Agent.TaskManager.{agent.Name}");

    public async Task<SendTaskResponse> SendTask(SendTaskRequest request)
    {
        using var activity = _trace.StartActivity($"TaskManager.{agent.Name}.{nameof(SendTask)}");

        var agentTask = request.CreateAgentTask();

        try
        {
            var textPart = request.Parameters.Message.Parts.First();
          
            var response = await agent.InvokeAsync(new ChatCompletionRequest { Message = textPart.Text, SessionId = request.Parameters.SessionId});
    
            var actionResponse = JsonSerializer.Deserialize<AgentActionResponse>(response.Message);
           
            if (actionResponse?.Action == AgentTaskState.InputRequired)
            {
                agentTask.SetInputRequiredState(actionResponse.Message);
            }

            if (actionResponse?.Action == AgentTaskState.Completed)
            {
                agentTask.SetCompletedState(actionResponse.Message);
            }

            if (actionResponse?.Action == AgentTaskState.Failed)
            {
                agentTask.SetInputRequiredFailed(actionResponse.Message);
            }

            return new SendTaskResponse { Task = agentTask };
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Agent Task Manager : {e.Message}");
            
            agentTask.SetInputRequiredFailed("We are not able to process your request at this time!");

            return new SendTaskResponse {  Task = agentTask };

        }
    }
}

public interface IAgentTaskManager
{
    Task<SendTaskResponse> SendTask(SendTaskRequest request);
}