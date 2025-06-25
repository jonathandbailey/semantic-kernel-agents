namespace Agents.Graph
{
    public class AgentTaskCompleteEdge(string targetNode) : IEdge
    {
        public string TargetNode { get; set; } = targetNode;
        public bool CanInvoke(AgentState state)
        {
            return AgentHeaderParser.HasHeader(AgentHeaderParser.TaskCompleteHeader, state.Response.Content!);
        }
    }
}
