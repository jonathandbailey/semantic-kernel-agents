using System.Text.Json;
using Microsoft.Extensions.Logging;
using Todo.Agents.Communication;
using Todo.Core.A2A;
using Todo.Infrastructure;

namespace Todo.Agents.Middleware
{
    public class Agent2AgentMiddleWare(ILogger<IAgent> logger, string agentName, IAgentPublisher publisher) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            var agentTaskRequest = GetAgentResponse(state);

            var agentTask = state.Get<AgentTask>("AgentTask");

            var sendTaskRequest = AgentExtensions.CreateSendTaskRequest(agentTask.TaskId,
                agentTask.SessionId, agentTaskRequest.Message);

            sendTaskRequest.AgentName = agentTaskRequest.AgentName;

            var response = await publisher.Send(sendTaskRequest);

            agentTask.SetTaskState(response.Task);

            return await next(state);
        }

        private AgentTaskRequest GetAgentResponse(AgentState state)
        {
            var message = state.Responses.First().Content;
            
            var agentResponse = JsonSerializer.Deserialize<AgentTaskRequest>(message!);

            if (agentResponse == null)
            {
                logger.LogError($"{agentName} Failed to deserialize agent response: {message}");

                throw new AgentException($"{agentName} :Failed to deserialize agent response: {message}");
            }

            return agentResponse;
        }
    }
}
