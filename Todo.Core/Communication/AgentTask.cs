namespace Todo.Core.Communication
{
    public class AgentTask
    {
        public List<AgentMessage> History {get; } = [];

        public List<AgentArtifact> Artifacts { get; } = [];
    }
}
