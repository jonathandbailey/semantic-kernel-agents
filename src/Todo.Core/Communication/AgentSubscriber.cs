using MediatR;
using Todo.Application.Agents.A2A;
using Todo.Application.Agents.Build;

namespace Todo.Application.Communication
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
