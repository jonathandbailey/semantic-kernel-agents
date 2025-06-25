namespace Agents.Graph
{
    public static class AgentGraphExtensions
    {
        public static AgentGraph Connect(this AgentGraph graph, string source, string target)
        {
            graph.AddEdge(source, new AgentInvokeEdge(target));

            return graph;
        }

        public static AgentGraph Connect(this AgentGraph graph, string source, IEdge target)
        {
            graph.AddEdge(source, target);

            return graph;
        }
    }
}
