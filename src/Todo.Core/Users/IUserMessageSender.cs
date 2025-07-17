namespace Todo.Core.Users;

public interface IUserMessageSender
{
    Task RespondAsync<T>(T payload, Guid userId);
    void Initialize(Guid sessionId, Guid userId);
    Task StreamingMessageCallback(string content, bool isEndOfStream, Guid id);
    Task CommandAsync<T>(T payload);
}