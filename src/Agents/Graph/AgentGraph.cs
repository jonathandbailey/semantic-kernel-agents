namespace Agents.Graph
{
    public class AgentGraph
    {
        private readonly Dictionary<string, INode> _nodes = new();

        private readonly Dictionary<string, List<IEdge>> _edges = new();

        public void AddNode(INode node)
        {
            _nodes[node.Name] = node;
        }

        public void AddEdge(string targetNode, List<IEdge> edge)
        {
            _edges[targetNode] = edge;
        }

        public async Task<NodeState> RunAsync(string startNode, NodeState nodeState)
        {
            var currentNode = startNode;

            var agentState = nodeState.AgentState;

            while (currentNode != null)
            {
                var node = _nodes[currentNode];

                agentState = await node.InvokeAsync(agentState);

                if (!_edges.TryGetValue(currentNode, out var outgoing))
                    break;

                var next = outgoing.FirstOrDefault(e => e.CanInvoke(agentState));
                currentNode = next?.TargetNode;
            }

            return new NodeState(agentState);
        }
    }
}
