namespace Todo.Application.Dto
{
    public class UserRequestDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string TaskId { get; set; } = string.Empty;
    }
}
