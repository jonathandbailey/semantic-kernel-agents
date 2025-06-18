namespace Todo.Core.Vacations;

public interface IVacationPlanStage
{
    PlanStatus Status { get; }
    string Description { get; }
}