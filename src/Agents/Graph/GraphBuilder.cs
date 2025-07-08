namespace Agents.Graph
{
    public class GraphBuilder
    {
        private readonly GraphRegistry _registry = new();
        private readonly AgentGraph _graph = new();

        public void AddNode(string name, IAgent agent)
        {
            var node = new AgentNode2(name, agent, _registry);

            _graph.AddNode(node);

            _registry.Nodes.Add(agent.Name, node.Name);
        }

        public void AddNode(INode node)
        {
            _graph.AddNode(node);
        }

        public void Connect(string source, string target)
        {
            _graph.AddEdge(source, new AgentInvokeEdge(target));
        }

        public AgentGraph Build()
        {
            return _graph;
        }
    }
}
