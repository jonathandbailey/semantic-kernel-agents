using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Todo.Infrastructure;

namespace Todo.Agents.Middleware
{
    public class AgentExceptionHandlingMiddleware(ILogger<IAgent> logger, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
        {
            try
            {
                var response = await next(context);

                return response;
            }
            catch (HttpOperationException exception)
            {
                logger.LogError($"HTTP Operation Error in Agent : {agentName} :{exception.Message}", exception);
                throw new AgentException("An error occurred during HTTP operation.", exception, exception.StatusCode);
            }
            
            catch (Exception e)
            {   
                
                logger.LogError($"Unexpected error in Agent : {agentName} :{e.Message}", e);
                throw new AgentException("An unexpected error occurred.", e);
            }
        }
    }
}
