namespace Agents.Graph
{
    public class AgentTaskCompleteEdge(string targetNode) : IEdge
    {
        public string TargetNode { get; set; } = targetNode;
        public bool CanInvoke(NodeState state)
        {
            return AgentHeaderParser.HasHeader(AgentHeaderParser.TaskCompleteHeader, state.AgentState.Response.Content!);
        }
    }
}
