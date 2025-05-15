namespace Todo.Core.Agents.A2A
{
    public class TaskSendParameters
    {
        public string SessionId { get; set; } = string.Empty;

        public Message Message { get; init; } = new Message();
    }
}
