namespace Todo.Core.Agents;

public interface IAgent
{
    public Task<AgentState> InvokeAsync(AgentState request);
    string Name { get; }
}