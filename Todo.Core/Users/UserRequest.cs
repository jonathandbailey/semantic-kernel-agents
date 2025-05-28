using MediatR;
using Todo.Core.Agents.A2A;

namespace Todo.Core.Users
{
    public class UserRequest : IRequest<UserResponse>
    {
        public string Message { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public required SendTaskRequest SendTaskRequest { get; init; }
    }
}
