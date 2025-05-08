using MediatR;

namespace Todo.Core.Messaging;

public interface IMessage : INotification
{
    public string Message { get; }
}