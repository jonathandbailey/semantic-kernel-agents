using Todo.Agents.Middleware;

namespace Todo.Agents
{
    public abstract class AgentBase : IAgentMiddleware
    {
        public abstract Task<AgentState> InvokeAsync(AgentState request);

        public async Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
        {
            var response = await InvokeAsync(context);

            return await next(response);
        }
    }
}
