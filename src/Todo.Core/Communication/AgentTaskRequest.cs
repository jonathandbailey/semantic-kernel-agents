namespace Todo.Application.Communication
{
    internal class AgentTaskRequest
    {
        public required string AgentName { get; init; } = string.Empty;
        public required string Message { get; init; } = string.Empty;
    }
}
