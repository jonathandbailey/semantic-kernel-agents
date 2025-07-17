namespace Todo.Core.Users;

public class AssistantCommand
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public Guid VacationPlanId { get; set; }
}