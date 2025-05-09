using Todo.Core.Agents;
using Todo.Core.Communication;

namespace Todo.Core.Middleware
{
    public class AgentDelegateWrapper(AgentDelegate agentDelegate) : IAgent
    {
        public async Task<AgentTask> InvokeAsync(AgentTask agentTask)
        {
            return await agentDelegate(agentTask);
        }
    }
}
