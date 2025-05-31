using Todo.Core.Agents.Middleware;

namespace Todo.Core.Agents
{
    public abstract class AgentBase : IAgentMiddleware
    {
        public abstract Task<AgentState> InvokeAsync(AgentState request);

        public async Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
        {
            return await InvokeAsync(context);
        }
    }
}
