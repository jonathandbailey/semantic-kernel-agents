namespace Todo.ConsoleApp.Dto
{
    public class UserResponseDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string TaskId { get; set; } = string.Empty;

        public bool HasError { get; set; } = false;
    }
}
