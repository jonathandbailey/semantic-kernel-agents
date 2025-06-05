namespace Todo.Core.A2A
{
    public class TaskSendParameters
    {
        public string Id { get; set; } = string.Empty;
        
        public string SessionId { get; set; } = string.Empty;

        public Message Message { get; init; } = new Message();
    }
}
