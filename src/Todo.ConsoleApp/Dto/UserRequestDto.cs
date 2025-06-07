namespace Todo.ConsoleApp.Dto
{
    public class UserRequestDto
    {
        public string SessionId { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public string TaskId { get; init; } = string.Empty;
    }
}
