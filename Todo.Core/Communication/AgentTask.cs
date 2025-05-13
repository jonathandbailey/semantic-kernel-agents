namespace Todo.Core.Communication
{
    public class AgentTask
    {
        public List<Message> History {get; } = [];

        public List<AgentArtifact> Artifacts { get; } = [];

        public string TaskId { get; init; } = string.Empty;

        public string SessionId { get; init; } = string.Empty;

        public TaskStatus Status { get; set; } = new TaskStatus();
    }
}
