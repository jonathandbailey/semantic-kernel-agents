using Todo.Application.Dto;
using Todo.Application.Users;
using Todo.Core.A2A;

namespace Todo.Api.Extensions
{
    public static class ModelExtensions
    {
        public static UserRequest ToUserRequest(this UserRequestDto userRequestDto, Guid userId)
        {
            var sendTaskRequest = new SendTaskRequest
            {
                Parameters = new TaskSendParameters
                {
                    Id = userRequestDto.TaskId,
                    SessionId = userRequestDto.SessionId,
                    Message = new Message
                    {
                        Parts = [new TextPart { Text = userRequestDto.Message }],
                        Role = "user"
                    }
                }
            };

            return new UserRequest
            {
                UserId = userId, 
                SendTaskRequest = sendTaskRequest,
                Message = userRequestDto.Message,
                SessionId = userRequestDto.SessionId,
                TaskId = userRequestDto.TaskId,
                VacationPlanId = userRequestDto.VacationPlanId,
                Source = userRequestDto.Source
            };
        }
    }
}
