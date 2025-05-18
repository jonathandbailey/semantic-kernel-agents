using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using Todo.Core.Agents.A2A;
using Todo.Core.Communication;
using Todo.Core.Extensions;
using Todo.Core.Infrastructure;

namespace Todo.Core.Agents;

public class AgentTaskManager(IAgent agent, ILogger<AgentTaskManager> logger, IAgentTaskRepository agentTaskRepository) : IAgentTaskManager
{
    private readonly ActivitySource _trace = new($"Todo.Agent.TaskManager.{agent.Name}");

    public async Task<SendTaskResponse> SendTask(SendTaskRequest request)
    {
        using var activity = _trace.StartActivity($"TaskManager.{agent.Name}.{nameof(SendTask)}");
  
        AgentTask agentTask;

        if (string.IsNullOrWhiteSpace(request.Parameters.Id))
        {
            agentTask = request.CreateAgentTask();
        }
        else
        {
            agentTask = await agentTaskRepository.LoadAsync(request.Parameters.Id);
        }
       

        try
        {
            var textPart = request.Parameters.Message.Parts.First();
          
            var response = await agent.InvokeAsync(new ChatCompletionRequest { Message = textPart.Text, SessionId = request.Parameters.SessionId});
    
            var actionResponse = JsonSerializer.Deserialize<AgentActionResponse>(response.Message);

            if (actionResponse == null)
            {
                logger.LogError($"{agent.Name} Task Manager failed to deserialize response from Agent.");

                agentTask.SetInputRequiredFailed("We are not able to process your request at this time!");

                return new SendTaskResponse { Task = agentTask };
            }

            agentTask.SetTaskState(actionResponse);

            await agentTaskRepository.SaveAsync(agentTask);

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