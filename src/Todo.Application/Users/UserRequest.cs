using MediatR;
using Todo.Application.Dto;

namespace Todo.Application.Users
{
    public class UserRequest : IRequest<UserResponseDto>
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; init; }

        public required string Message { get; set; }

        public string? TaskId { get; set; }

        public Guid SessionId { get; set; } = Guid.Empty;

        public Guid VacationPlanId { get; set; } = Guid.Empty;

        public string Source { get; set; } = string.Empty;
    }
}
