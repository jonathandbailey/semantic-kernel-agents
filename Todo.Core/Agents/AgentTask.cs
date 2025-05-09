namespace Todo.Core.Agents
{
    public class AgentTask
    {
        public List<AgentMessage> History {get; } = [];

        public List<AgentArtifact> Artifacts { get; } = [];
    }
}
