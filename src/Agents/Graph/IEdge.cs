namespace Agents.Graph;

public interface IEdge
{
    string TargetNode { get; set; }
    bool CanInvoke(AgentState state);
}