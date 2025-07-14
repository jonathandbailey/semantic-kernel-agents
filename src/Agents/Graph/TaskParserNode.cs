
namespace Agents.Graph;

public class TaskParserNode(string name) : INode
{
    public Task<NodeState> InvokeAsync(NodeState state)
    {
        state.AgentState.Arguments.Add("StageTasks", state.AgentState.Response.Content!);

        state.Route = NodeNames.Task;

        return Task.FromResult(state);
    }

    public string Name { get; } = name;
    public Guid Id { get; } = Guid.NewGuid();
}