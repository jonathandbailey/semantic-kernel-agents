using System.Text.Json;
using Microsoft.Extensions.Logging;
using Todo.Agents.Communication;
using Todo.Infrastructure;

namespace Todo.Agents.Middleware
{
    public class AgentResponseMiddleware(ILogger<IAgent> logger, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
        {
            var response = GetAgentResponse(context.ChatCompletionResponse);

            context.AgentTask.SetTaskState(response);

            return await next(context);
        }

        private AgentActionResponse GetAgentResponse(ChatCompletionResponse? chatCompletionResponse)
        {
            var agentResponse = JsonSerializer.Deserialize<AgentActionResponse>(chatCompletionResponse!.Message);

            if (agentResponse == null)
            {
                logger.LogError($"{agentName} Failed to deserialize agent response: {chatCompletionResponse.Message}");

                throw new AgentException($"{agentName} :Failed to deserialize agent response: {chatCompletionResponse.Message}");
            }

            return agentResponse;
        }
    }
}
