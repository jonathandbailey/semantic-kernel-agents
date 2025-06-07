namespace Todo.Agents;

public interface IAgent
{
    public Task<AgentState> InvokeAsync(AgentState state);
    string Name { get; }
}