using MediatR;

namespace Todo.Core.Communication
{
    public class SendTaskRequest : IRequest<SendTaskResponse>
    {
        public TaskSendParameters Parameters { get; init; } = new TaskSendParameters();
    }
}
