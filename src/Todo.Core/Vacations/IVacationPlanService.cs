namespace Todo.Core.Vacations;

public interface IVacationPlanService
{
    Task<VacationPlan> GetAsync(Guid id);
    Task UpdateAsync(VacationPlan vacationPlan, string source);
    Task UpdateItemAsync(Guid vacationPlanId, Guid taskId);
    Task UpdateItemAsync(Guid vacationPlanId, Guid taskId, List<StageTask> stageTasks);
    Task<VacationPlan> Create();
}