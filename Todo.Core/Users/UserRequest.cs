using MediatR;
using Todo.Core.Agents.A2A;

namespace Todo.Core.Users
{
    public class UserRequest : IRequest<UserResponse>
    {
        public string SessionId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public required SendTaskRequest SendTaskRequest { get; init; }
    }
}
