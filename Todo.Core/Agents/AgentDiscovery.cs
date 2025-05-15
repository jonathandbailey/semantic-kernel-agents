using Todo.Core.Agents.A2A;

namespace Todo.Core.Agents
{
    public class AgentDiscovery : IAgentDiscovery
    {
        public Task<List<AgentCard>> Get()
        {
            return Task.FromResult(new List<AgentCard>());
        }
    }
    public interface IAgentDiscovery
    {
        Task<List<AgentCard>> Get();
    }
}
