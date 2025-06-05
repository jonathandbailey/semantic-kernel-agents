using MediatR;

namespace Todo.Application.Agents.A2A
{
    public class SendTaskRequest : IRequest<SendTaskResponse>
    {
        public string AgentName { get; set; } = string.Empty;

        public TaskSendParameters Parameters { get; init; } = new TaskSendParameters();
    }
}
