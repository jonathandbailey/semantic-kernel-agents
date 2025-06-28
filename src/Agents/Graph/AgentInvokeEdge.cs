namespace Agents.Graph
{
    public class AgentInvokeEdge(string targetNode) : IEdge
    {
        public string TargetNode { get; set; } = targetNode;

        public bool CanInvoke(NodeState state)
        {
            var headerMatch = $"[agent-invoke:{TargetNode}]";

            return AgentHeaderParser.HasHeader(headerMatch, state.Headers);
        }
    }
}
