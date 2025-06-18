using Todo.Core.A2A;
using Todo.Core.Vacations;
using Todo.Infrastructure.File;

namespace Todo.Application.Services;

public class VacationPlanService(IVacationPlanRepository vacationPlanRepository) : IVacationPlanService
{
    public async Task<VacationPlan> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            var vacationPlan = new VacationPlan(Guid.NewGuid(), PlanStatus.Open, "New Vacation Plan", "No description available.");

            vacationPlan.AddStage(new TravelPlan(Guid.NewGuid(), "Travel Plan", "No description available", PlanStage.Travel, PlanStatus.Open));

            return vacationPlan;
        }

        return await vacationPlanRepository.Load(id);
    }

    public async Task UpdateAsync(VacationPlan vacationPlan, string source, string agentTaskState)
    {
        if (agentTaskState == AgentTaskState.Completed)
        {
            if (source == "TravelAgent")
            {
                vacationPlan.UpdateStageStatus(PlanStage.Travel, PlanStatus.Completed);
            }

            await vacationPlanRepository.Save(vacationPlan);
        }
    }
}

public interface IVacationPlanService
{
    Task<VacationPlan> GetAsync(Guid id);
    Task UpdateAsync(VacationPlan vacationPlan, string source, string agentTaskState);
}