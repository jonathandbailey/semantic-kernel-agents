using Agents.Build;

namespace Agents.Graph.v2
{
    public class AgentNode(string name, Guid id, Registry registry, IAgentProvider agentProvider) : BaseNode, INode
    {
        public async Task<NodeState> InvokeAsync(NodeState state)
        {
            var agent = await agentProvider.Create(Name);

            var agentState = await agent.InvokeAsync(state.AgentState);
            
            return new NodeState(agentState);
        }

        public override Guid Id { get; } = id;
        public override string Name { get; } = name;
    }
}
