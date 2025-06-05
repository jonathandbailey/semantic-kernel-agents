namespace Todo.Application.Agents.A2A
{
    public class Message
    {
        public string Role { get; set; } = string.Empty;

        public List<TextPart> Parts { get; set; } = new List<TextPart>();
    }
}
