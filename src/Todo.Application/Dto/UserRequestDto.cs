using System.ComponentModel.DataAnnotations;

namespace Todo.Application.Dto
{
    public class UserRequestDto
    {
        public string SessionId { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public string TaskId { get; set; } = string.Empty;

        public Guid VacationPlanId { get; set; } = Guid.Empty;

        public string Source { get; set; } = string.Empty;
    }
}
