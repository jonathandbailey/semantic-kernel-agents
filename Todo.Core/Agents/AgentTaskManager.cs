using System.Text.Json;
using Microsoft.Extensions.Logging;
using Todo.Core.Agents.A2A;
using Todo.Core.Communication;
using Todo.Core.Extensions;

namespace Todo.Core.Agents;

public class AgentTaskManager(IAgent agent, ILogger<AgentTaskManager> logger) : IAgentTaskManager
{
    public async Task<SendTaskResponse> SendTask(SendTaskRequest request)
    {
        try
        {
            var textPart = request.Parameters.Message.Parts.First();

            var agentTask = request.CreateAgentTask();
          
            var response = await agent.InvokeAsync(new ChatCompletionRequest { Message = textPart.Text, SessionId = request.Parameters.SessionId});
    
            var actionResponse = JsonSerializer.Deserialize<AgentActionResponse>(response.Message);
            
            if (actionResponse?.Action == "User_Input_Required")
            {
                agentTask.SetInputRequiredState(actionResponse.Message);
            }

            if (actionResponse?.Action == "Complete")
            {
                agentTask.SetCompletedState(actionResponse.Message);
            }

            return new SendTaskResponse { Message = response.Message, Task = agentTask };
        }
        catch (Exception e)
        {
            logger.LogError($"Agent Task Manager : {e.Message}", e);
            throw;
        }
    }
}

public interface IAgentTaskManager
{
    Task<SendTaskResponse> SendTask(SendTaskRequest request);
}