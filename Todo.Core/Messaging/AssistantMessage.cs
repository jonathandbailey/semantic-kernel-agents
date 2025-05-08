namespace Todo.Core.Messaging
{
    public class AssistantMessage(string message) : IMessage
    {
        public string Message { get; } = message;
    }
}
