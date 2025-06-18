using Microsoft.Extensions.Logging;
using Todo.Core.A2A;
using Todo.Infrastructure.Azure;

namespace Todo.Agents.Middleware;

public class AgentTaskMiddleware(ILogger<IAgent> logger, IAgentTaskRepository agentTaskRepository) : IAgentMiddleware
{
    public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
    {
        var agentTask = await GetOrCreateAgentTask(state);

        try
        {
            state.Add(agentTask);

            state = await next(state);

            agentTask = state.GetAgentTask();
       
            agentTask.UpdateTaskState(state);

            await agentTaskRepository.SaveAsync(agentTask);

            state.SetTaskId(agentTask.TaskId);
            state.SetSessionId(agentTask.SessionId);
            
            return state;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Task Middleware : {e.Message}");

            agentTask.SetInputRequiredFailed("We are not able to process your request at this time!");

            return state;
        }
    }
    
    private async Task<AgentTask> GetOrCreateAgentTask(AgentState state)
    {
        var taskId = state.GetTaskId();
        var sessionId = state.GetSessionId();
        
        if (string.IsNullOrWhiteSpace(taskId))
        {
            return AgentExtensions.CreateAgentTask(state.Request.Content!);
        }

        if (await agentTaskRepository.Exists(taskId))
        {
            var agentTask = await agentTaskRepository.LoadAsync(taskId);

            if (!string.IsNullOrEmpty(sessionId))
            {
                agentTask.SessionId = sessionId;
            }

            return agentTask;
        }

        var newTask = AgentExtensions.CreateAgentTask(state.Request.Content!);

        newTask.TaskId = taskId;

        if (!string.IsNullOrEmpty(sessionId))
        {
            newTask.SessionId = sessionId;
        }

        return newTask;
    }
}