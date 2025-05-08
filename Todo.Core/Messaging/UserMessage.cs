using MediatR;

namespace Todo.Core.Messaging
{
    public class UserMessage : INotification
    {
        public string Message { get; set; } = string.Empty;
    }
}
