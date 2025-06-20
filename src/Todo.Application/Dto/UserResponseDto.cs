namespace Todo.Application.Dto
{
    public class UserResponseDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string TaskId { get; set; } = string.Empty;

        public Guid VacationPlanId { get; set; } = Guid.Empty;

        public bool IsEndOfStream { get; set; } = false;

        public string Source { get; set; } = string.Empty;
    }
}
