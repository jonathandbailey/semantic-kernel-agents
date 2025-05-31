using MediatR;
using Todo.Core.Agents.A2A;
using Todo.Core.Agents.Build;

namespace Todo.Core.Communication
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
