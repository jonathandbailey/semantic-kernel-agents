namespace Agents.Graph.v2;

public abstract class BaseNode
{
    public List<IEdge> Edges { get; } = new();

    public abstract Guid Id { get; }

    public abstract string Name { get; }

    public void AddEdge(IEdge edge)
    {
        Edges.Add(edge);
    }
}