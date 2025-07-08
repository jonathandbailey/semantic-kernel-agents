namespace Agents.Graph
{
    public class DefaultEdge(string targetNode) : IEdge
    {
        public string TargetNode { get; set; } = targetNode;
        public bool CanInvoke(NodeState state)
        {
            return true;
        }
    }
}
