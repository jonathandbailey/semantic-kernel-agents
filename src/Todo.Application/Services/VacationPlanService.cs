﻿using Todo.Core.Vacations;
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

    public async Task UpdateItemAsync(Guid vacationPlanId, Guid taskId)
    {
        var vacationPlan = await vacationPlanRepository.Load(vacationPlanId);

        vacationPlan.UpdateStageStatus(taskId, PlanStatus.Completed);

        await vacationPlanRepository.Save(vacationPlan);
    }

    public async Task UpdateItemAsync(Guid vacationPlanId, Guid taskId, List<StageTask> stageTasks)
    {
        var vacationPlan = await vacationPlanRepository.Load(vacationPlanId);

        vacationPlan.UpdateStageStatus(taskId, PlanStatus.Completed, stageTasks);

        await vacationPlanRepository.Save(vacationPlan);
    }

    public async Task<VacationPlan> Create()
    {
        var vacationPlan = new VacationPlan(Guid.NewGuid(), PlanStatus.Open, "Untitled", "No description available.");

        vacationPlan.AddStage(new TravelPlan(Guid.NewGuid(), "Accommodation", "All tasks related to booking accommodation.", PlanStage.Accommodation, PlanStatus.Open));
        vacationPlan.AddStage(new TravelPlan(Guid.NewGuid(), "Travel", "All tasks related to getting to and from the destination.", PlanStage.Travel, PlanStatus.Open));

        await vacationPlanRepository.Save(vacationPlan);

        await vacationPlanRepository.AddVacationPlanToCatalog(vacationPlan);

        return vacationPlan;
    }
}