namespace Todo.Application.Dto
{
    public class AssistantCommand
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid VacationPlanId { get; set; }
    }
}
