namespace Todo.Core.Vacations;

public interface IVacationPlanService
{
    Task<VacationPlan> GetAsync(Guid id);
    Task UpdateAsync(VacationPlan vacationPlan, string source);
}