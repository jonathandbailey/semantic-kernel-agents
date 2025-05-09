using Todo.Core.Communication;

namespace Todo.Core.Agents;

public interface IAgent
{
    Task<AgentTask> InvokeAsync(AgentTask agentTask);
}