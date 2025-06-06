using MediatR;
using Todo.Agents.Build;
using Todo.Core.A2A;

namespace Todo.Agents.Communication
{
    public class AgentSubscriber(IAgentProvider agentProvider) : IRequestHandler<SendTaskRequest, SendTaskResponse>
    {
        public async Task<SendTaskResponse> Handle(SendTaskRequest request, CancellationToken cancellationToken)
        {
            var agentTaskManager = agentProvider.GetTaskManager(request.AgentName);
           
            var response = await agentTaskManager.SendTask(request);

            return response;
        }
    }
}
