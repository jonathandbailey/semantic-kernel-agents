using Todo.Application.Communication;
using Todo.Core.A2A;

namespace Todo.Application.Agents;

public class AgentStateStore : IAgentStateStore
{
    private readonly Dictionary<string, AgentState> _agentState = new();

    public AgentState Get(string agentName)
    {
        if (_agentState.TryGetValue(agentName, out var agentState))
        {
            return agentState;
        }

        throw new InvalidOperationException($"Agent state {agentName} does not exist.");
    }

    public AgentState Update(string agentName, string sessionId, string taskId, AgentTask agentTask)
    {
        if (_agentState.TryGetValue(agentName, out var agentState))
        {
            agentState.SessionId = sessionId;
            agentState.TaskId = taskId;
        }
        else
        {
            _agentState[agentName] = new AgentState { SessionId = sessionId, TaskId = taskId, ChatCompletionRequest = new ChatCompletionRequest(),AgentTask = agentTask};
        }

        return _agentState[agentName];
    }
}

public interface IAgentStateStore
{
    AgentState Get(string agentName);
    AgentState Update(string agentName, string sessionId, string taskId, AgentTask agentTask);
}