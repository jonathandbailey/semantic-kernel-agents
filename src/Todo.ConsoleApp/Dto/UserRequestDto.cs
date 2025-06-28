namespace Todo.ConsoleApp.Dto
{
    public class UserRequestDto
    {
        public Guid SessionId { get; init; } = Guid.Empty;

        public string Message { get; init; } = string.Empty;

        public string TaskId { get; init; } = string.Empty;

        public Guid VacationPlanId { get; set; } = Guid.Empty;

        public string Source { get; set; } = string.Empty;
    }
}
