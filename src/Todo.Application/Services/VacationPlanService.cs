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

            vacationPlan.AddStage(new TravelPlan(Guid.NewGuid(), "Accommodation", "All tasks related to booking accommodation.", PlanStage.Accommodation, PlanStatus.Open));
            vacationPlan.AddStage(new TravelPlan(Guid.NewGuid(), "Travel", "All tasks related to getting to and from the destination.", PlanStage.Travel, PlanStatus.Open));
            vacationPlan.AddStage(new TravelPlan(Guid.NewGuid(), "Budget", "All tasks related to budget planning for the trip.", PlanStage.Budget, PlanStatus.Open));

            await vacationPlanRepository.Save(vacationPlan);

            return vacationPlan;
        }

        return await vacationPlanRepository.Load(id);
    }

    public async Task UpdateAsync(VacationPlan vacationPlan, string source)
    {
        
            if (source == "Travel")
            {
                vacationPlan.UpdateStageStatus(PlanStage.Travel, PlanStatus.Completed);
            }

            if (source == "Accommodation")
            {
                vacationPlan.UpdateStageStatus(PlanStage.Accommodation, PlanStatus.Completed);
            }

            await vacationPlanRepository.Save(vacationPlan);
        
    }
}