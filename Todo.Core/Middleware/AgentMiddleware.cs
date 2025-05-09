using Todo.Core.Agents;

namespace Todo.Core.Middleware
{
    public class AgentMiddleware(IAgent agent) : IAgentMiddleware
    {
        public async Task<AgentTask> InvokeAsync(AgentTask context, AgentDelegate next)
        {
            return await agent.InvokeAsync(context);
        }
    }
}
