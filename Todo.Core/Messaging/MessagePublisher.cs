using MediatR;

namespace Todo.Core.Messaging
{
    public class MessagePublisher(IMediator mediator) : IMessagePublisher
    {
        public async Task Publish(IMessage message)
        {
            await mediator.Publish(message);
        }
    }

    public interface IMessagePublisher
    {
        Task Publish(IMessage message);
    }
}
