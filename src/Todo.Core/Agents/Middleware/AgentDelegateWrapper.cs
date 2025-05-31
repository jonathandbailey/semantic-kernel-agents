namespace Todo.Core.Agents.Middleware;

public class AgentDelegateWrapper(AgentDelegate agentDelegate, string name) : IAgent
{
    public async Task<AgentState> InvokeAsync(AgentState request)
    {
        return await agentDelegate(request);
    }

    public string Name { get; } = name;
}