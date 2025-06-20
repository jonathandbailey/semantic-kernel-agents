namespace Agents.Middleware
{
    public class TerminationMiddleWare : IAgentMiddleware
    {
        public Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            return Task.FromResult(state);
        }
    }
}
