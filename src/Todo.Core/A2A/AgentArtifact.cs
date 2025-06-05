namespace Todo.Core.A2A
{
    public class AgentArtifact
    {
        public string Name { get; set; } = string.Empty;

        public List<TextPart> Parts { get; set; } = new List<TextPart>();
    }
}
