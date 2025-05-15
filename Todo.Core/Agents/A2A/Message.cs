namespace Todo.Core.Agents.A2A
{
    public class Message
    {
        public string Role { get; set; } = string.Empty;

        public List<TextPart> Parts { get; init; } = new List<TextPart>();
    }
}
