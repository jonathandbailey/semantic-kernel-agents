namespace Agents.Graph.v2;

public interface INode
{
    Task<NodeState> InvokeAsync(NodeState state);

    Guid Id { get; }

    string Name { get; }

    List<IEdge> Edges { get; }

    void AddEdge(IEdge edge);
}