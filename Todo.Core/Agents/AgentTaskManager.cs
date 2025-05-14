using System.Text.Json;
using Microsoft.Extensions.Logging;
using Todo.Core.Communication;
using TaskStatus = Todo.Core.Communication.TaskStatus;

namespace Todo.Core.Agents;

public class AgentTaskManager(IAgent agent, ILogger<AgentTaskManager> logger) : IAgentTaskManager
{
    public async Task<SendTaskResponse> SendTask(SendTaskRequest request)
    {
        try
        {
            var textPart = request.Parameters.Message.Parts.First();

            var agentTask = new AgentTask
            {
                SessionId = request.Parameters.SessionId,
                TaskId = Guid.NewGuid().ToString(),
           
            };

            agentTask.History.Add(request.Parameters.Message);

            var response = await agent.InvokeAsync(new ChatCompletionRequest { Message = textPart.Text, SessionId = request.Parameters.SessionId});
    
            var actionResponse = JsonSerializer.Deserialize<AgentActionResponse>(response.Message);
            
            if (actionResponse?.Action == "User_Input_Required")
            {
                agentTask.Status = new TaskStatus
                {
                    State = "input-required",
                    Message = new Message
                    {
                        Role = "agent",
                        Parts = [new TextPart { Text = actionResponse.Message }]
                    }
                };
            }

            if (actionResponse?.Action == "Complete")
            {
                agentTask.Status = new TaskStatus
                {
                    State = "completed"
                };

                agentTask.Artifacts.Add(new AgentArtifact
                {
                    Parts = [new TextPart { Text = actionResponse.Message }],
                });
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