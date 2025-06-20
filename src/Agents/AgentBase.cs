using Agents.Middleware;

namespace Agents
{
    public abstract class AgentBase : IAgentMiddleware
    {
        public abstract Task<AgentState> InvokeAsync(AgentState state);

        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            var response = await InvokeAsync(state);

            return await next(response);
        }
    }
}
