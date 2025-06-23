namespace Agents.Graph;

public interface INode
{
    public Task<AgentState> InvokeAsync(AgentState state);
    string Name { get; }
}