using MediatR;

namespace Todo.Core.Users
{
    public class UserRequest : IRequest<UserResponse>
    {
        public string SessionId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
