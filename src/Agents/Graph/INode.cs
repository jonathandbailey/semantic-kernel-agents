namespace Agents.Graph;

public interface INode
{
    public Task<NodeState> InvokeAsync(NodeState state);
    string Name { get; }
    Guid Id { get; }
}