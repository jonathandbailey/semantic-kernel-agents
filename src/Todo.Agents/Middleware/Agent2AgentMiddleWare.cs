using System.Text.Json;
using Microsoft.Extensions.Logging;
using Todo.Agents.Communication;
using Todo.Infrastructure;

namespace Todo.Agents.Middleware
{
    public class Agent2AgentMiddleWare(ILogger<IAgent> logger, string agentName, IAgentPublisher publisher) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            var agentTaskRequest = GetAgentResponse(state);

            var sendTaskRequest = AgentExtensions.CreateSendTaskRequest(state.AgentTask.TaskId,
                state.AgentTask.SessionId, agentTaskRequest.Message);

            sendTaskRequest.AgentName = agentTaskRequest.AgentName;

            var response = await publisher.Send(sendTaskRequest);

            state.AgentTask.SetTaskState(response.Task);

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
