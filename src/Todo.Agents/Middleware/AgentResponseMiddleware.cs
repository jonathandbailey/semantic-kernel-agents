using System.Text.Json;
using Microsoft.Extensions.Logging;
using Todo.Agents.Communication;
using Todo.Infrastructure;

namespace Todo.Agents.Middleware
{
    public class AgentResponseMiddleware(ILogger<IAgent> logger, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            foreach (var chatMessageContent in state.Responses)
            {
                var response = GetAgentResponse(chatMessageContent.Content!);

                state.AgentTask.SetTaskState(response);
            }
  
            return await next(state);
        }

        private AgentActionResponse GetAgentResponse(string message)
        {
            var agentResponse = JsonSerializer.Deserialize<AgentActionResponse>(message);

            if (agentResponse == null)
            {
                logger.LogError($"{agentName} Failed to deserialize agent response: {message}");

                throw new AgentException($"{agentName} :Failed to deserialize agent response: {message}");
            }

            return agentResponse;
        }
    }
}
