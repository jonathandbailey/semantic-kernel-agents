namespace Agents.Graph
{
    public class AgentInvokeEdge(string targetNode) : IEdge
    {
        public string TargetNode { get; set; } = targetNode;

        public bool CanInvoke(AgentState state)
        {
            var source = state.Get<string>("source");

            return source == TargetNode;
        }
    }
}
