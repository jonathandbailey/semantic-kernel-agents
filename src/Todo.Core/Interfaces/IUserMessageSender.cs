namespace Todo.Core.Interfaces;

public interface IUserMessageSender
{
    Task RespondAsync<T>(T payload, Guid userId);
}