using MediatR;
using Todo.Core.A2A;

namespace Todo.Application.Users
{
    public class UserRequest : IRequest<UserResponse>
    {
        public Guid UserId { get; set; }

        public required SendTaskRequest SendTaskRequest { get; init; }
    }
}
