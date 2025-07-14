using System.ComponentModel;
using Microsoft.SemanticKernel;
using System.Text;
using Agents.Build;
using Todo.Core.Vacations;

namespace Agents.Plugins
{
    public  class TravelTaskPlugin(IVacationPlanService vacationPlanService) : IAgentPlugin
    {
        [KernelFunction("get_travel_task_list")]
        [Description("Gets a list of the Users Travel Tasks")]
        public async Task<string> GetTravelTaskList(
           [Description("VacationPlanId")] Guid vacationPlanId)
        {
            var vacationPlan = await vacationPlanService.GetAsync(vacationPlanId);

            var stringBuilder = new StringBuilder();

            foreach (var vacationPlanStage in vacationPlan.Stages)
            {
                stringBuilder.AppendLine($"[{vacationPlanStage.Stage} - '{vacationPlanStage.Description}' :{vacationPlanStage.Status}]");
            }

            return stringBuilder.ToString();
        }

        [KernelFunction("complete_travel_task_item")]
        [Description("Updates a Task in a Vacation Travel Plan")]
        public async Task<string> UpdateTravelTaskItem(
            [Description("VacationPlanId")]  Guid vacationPlanId, 
            [Description("TaskId")] Guid taskId,
            [Description("StageTasks")] string stageTasks)
        {
            await vacationPlanService.UpdateItemAsync(vacationPlanId, taskId);

            return "Task Updated Successfully.";
        }
    }
}
