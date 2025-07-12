using Todo.Application.Dto;
using Todo.Application.Users;

namespace Todo.Api.Extensions
{
    public static class ModelExtensions
    {
        public static UserRequest ToUserRequest(this UserRequestDto userRequestDto, Guid userId)
        {
            var sessionId = Guid.TryParse(userRequestDto.SessionId, out var guid) ? guid : Guid.Empty;
            
            return new UserRequest
            {
                UserId = userId, 
                Message = userRequestDto.Message,
                SessionId = sessionId,
                TaskId = userRequestDto.TaskId,
                VacationPlanId = userRequestDto.VacationPlanId,
                Source = userRequestDto.Source,
                Id = userRequestDto.Id
                
            };
        }
    }
}
