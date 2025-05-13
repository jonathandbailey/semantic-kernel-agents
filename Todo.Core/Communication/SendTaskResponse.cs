namespace Todo.Core.Communication
{
    public class SendTaskResponse
    {
        public string Message { get; init; } = string.Empty;

        public AgentTask Task { get; init; } = new AgentTask();
    }
}
