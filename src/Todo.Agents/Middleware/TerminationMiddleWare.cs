namespace Todo.Agents.Middleware
{
    public class TerminationMiddleWare : IAgentMiddleware
    {
        public Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
        {
            return Task.FromResult(context);
        }
    }
}
