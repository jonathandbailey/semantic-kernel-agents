using MediatR;
using Todo.Application.Dto;
using Todo.Core.A2A;

namespace Todo.Application.Users
{
    public class UserRequest : IRequest<UserResponseDto>
    {
        public Guid UserId { get; set; }

        public required SendTaskRequest SendTaskRequest { get; init; }
    }
}
