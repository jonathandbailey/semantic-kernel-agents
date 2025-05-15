namespace Todo.Core.Agents.A2A
{
    public class AgentArtifact
    {
        public string Name { get; init; } = string.Empty;

        public List<TextPart> Parts { get; init; } = new List<TextPart>();
    }
}
