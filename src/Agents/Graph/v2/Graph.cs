namespace Agents.Graph.v2
{
    public class AgentGraph(Registry registry)
    {
        private readonly Registry _registry = registry;
        private readonly Dictionary<Guid, INode> _nodes = new();
        private readonly Dictionary<Guid, IEdge> _edges = new();
        
        public async Task<NodeState> RunAsync(INode node, NodeState nodeState)
        {
            var currentNode = node;

            var state = nodeState;

            while (currentNode != null)
            {
                state = await currentNode.InvokeAsync(state);

                var next = currentNode.Edges.FirstOrDefault(e => e.CanRoute(state));

                currentNode = next?.Target;
            }

            return state;
        }

        public void AddNode(INode node)
        {
            _nodes.Add(node.Id, node);

            _registry.Nodes.Add(node.Name, node.Id);
        }

        public void AddEdge(INode source, INode target)
        {
            var edge = new Edge(Guid.NewGuid(), target);

            source.AddEdge(edge);

            _edges.Add(edge.Id, edge);
        }
    }
}
