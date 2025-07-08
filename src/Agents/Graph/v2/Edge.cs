namespace Agents.Graph.v2;

public class Edge(Guid id, INode target) : BaseEdge, IEdge
{
    public override Guid Id { get; } = id;
    public override INode Target { get; } = target;
    public override bool CanRoute(NodeState nodeState)
    {
        return nodeState.Routing == Target.Id;
    }
}

public interface IEdge
{
    Guid Id { get; }

    INode Target { get; }

    bool CanRoute(NodeState nodeState);
}