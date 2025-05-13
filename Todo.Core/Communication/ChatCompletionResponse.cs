namespace Todo.Core.Communication
{
    public class ChatCompletionResponse
    {
        public string Message { get; init; } = string.Empty;

        public string SessionId { get; init; } = string.Empty;
    }
}
