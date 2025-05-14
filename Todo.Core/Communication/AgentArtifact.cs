namespace Todo.Core.Communication
{
    public class AgentArtifact
    {
        public string Name { get; init; } = string.Empty;

        public List<TextPart> Parts { get; init; } = new List<TextPart>();
    }
}
