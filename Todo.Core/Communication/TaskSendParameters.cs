namespace Todo.Core.Communication
{
    public class TaskSendParameters
    {
        public string SessionId { get; init; } = string.Empty;

        public Message Message { get; init; } = new Message();
    }
}
