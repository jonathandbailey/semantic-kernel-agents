namespace Todo.Core.Communication
{
    public class TaskStatus
    {
        public string State { get; set;  } = string.Empty;

        public Message Message { get; set; } = new Message();
    }
}
