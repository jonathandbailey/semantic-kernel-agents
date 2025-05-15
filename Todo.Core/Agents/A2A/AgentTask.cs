namespace Todo.Core.Agents.A2A
{
    public class AgentTask
    {
        public List<Message> History {get; } = [];

        public List<AgentArtifact> Artifacts { get; } = [];

        public string TaskId { get; init; } = string.Empty;

        public string SessionId { get; init; } = string.Empty;

        public AgentTaskStatus Status { get; set; } = new AgentTaskStatus();
    }
}
