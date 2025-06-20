using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Todo.Infrastructure;

namespace Agents.Middleware
{
    public class AgentExceptionHandlingMiddleware(ILogger<IAgent> logger, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            try
            {
                return await next(state);
            }
            catch (HttpOperationException exception)
            {
                var agentException = new AgentException("An error occurred during HTTP operation.", exception, exception.StatusCode);

                logger.LogError($"HTTP Operation Error in Agent : {agentName} :{exception.Message}", agentException);
                
                throw agentException;
            }
            
            catch (Exception e)
            {   
                var agentException =  new AgentException("An unexpected error occurred.", e);

                logger.LogError($"Unexpected error in Agent : {agentName} :{e.Message}", agentException);

                throw agentException;
            }
        }
    }
}
