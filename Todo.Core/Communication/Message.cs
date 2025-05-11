namespace Todo.Core.Communication
{
    public class Message
    {
        public string Role { get; init; } = string.Empty;

        public List<TextPart> Parts { get; init; } = new List<TextPart>();
    }
}
