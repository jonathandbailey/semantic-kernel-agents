namespace Todo.Core.Communication
{
    public class SendTaskRequest 
    {
        public TaskSendParameters Parameters { get; init; } = new TaskSendParameters();
    }
}
