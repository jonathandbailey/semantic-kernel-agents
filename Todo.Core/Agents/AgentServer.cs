using Todo.Core.Communication;

namespace Todo.Core.Agents
{
    public interface IAgentServer
    {
        Task SendTask(string agentName, SendTaskRequest sendTaskRequest);
    }

    public class AgentServer(IAgentProvider agentProvider) : IAgentServer
    {
        public async Task SendTask(string agentName, SendTaskRequest sendTaskRequest)
        {
            await agentProvider.GetTaskManager(agentName).SendTask(sendTaskRequest);
        }
    }
}
