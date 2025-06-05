using MediatR;
using Todo.Application.Agents.A2A;

namespace Todo.Application.Users
{
    public class UserRequest : IRequest<UserResponse>
    {
        public string Message { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public required SendTaskRequest SendTaskRequest { get; init; }
    }
}
