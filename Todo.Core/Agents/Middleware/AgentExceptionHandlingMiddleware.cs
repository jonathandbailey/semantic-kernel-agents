using Microsoft.Extensions.Logging;
using Todo.Core.Communication;

namespace Todo.Core.Agents.Middleware
{
    public class AgentExceptionHandlingMiddleware(ILogger<IAgent> logger, string agentName) : IAgentMiddleware
    {
        public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest context, AgentDelegate next)
        {
            try
            {
                var response = await next(context);

                return response;
            }
            catch (Exception e)
            {
                logger.LogError($"Unknown Error in Agent : {agentName} :{e.Message}", e);
                throw;
            }
        }
    }
}
