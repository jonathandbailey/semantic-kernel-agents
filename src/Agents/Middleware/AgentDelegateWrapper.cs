
using Agents.Graph;

namespace Agents.Middleware;

public class AgentDelegateWrapper(AgentDelegate agentDelegate, string name) : IAgent, INode
{
    public async Task<AgentState> InvokeAsync(AgentState state)
    {
        return await agentDelegate(state);
    }

    public string Name { get; } = name;
}