using Todo.Application.Dto;
using Todo.Application.Users;

namespace Todo.Api.Extensions
{
    public static class ModelExtensions
    {
        public static UserRequest ToUserRequest(this UserRequestDto userRequestDto, Guid userId)
        {
            return new UserRequest
            {
                UserId = userId, 
                Message = userRequestDto.Message,
                SessionId = userRequestDto.SessionId,
                TaskId = userRequestDto.TaskId,
                VacationPlanId = userRequestDto.VacationPlanId,
                Source = userRequestDto.Source
            };
        }
    }
}
