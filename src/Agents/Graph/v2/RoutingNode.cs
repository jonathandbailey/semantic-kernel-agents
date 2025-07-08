namespace Agents.Graph.v2
{
    public class RoutingNode(string name, Guid id, Guid target) : BaseNode, INode
    {
        public Task<NodeState> InvokeAsync(NodeState state)
        {
            if (state.Routing == Guid.Empty)
            {
                state.Routing = target;
            }

            return Task.FromResult(state);
        }

        public override Guid Id { get; } = id;
        public override string Name { get; } = name;
    }
}
