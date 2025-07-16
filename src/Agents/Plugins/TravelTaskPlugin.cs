using Agents.Build;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
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

            try
            {
                var tasks = JsonSerializer.Deserialize<List<StageTask>>(stageTasks);

                if (tasks == null)
                {
                    return "Task Failed to Deserailize.";
                }

                await vacationPlanService.UpdateItemAsync(vacationPlanId, taskId, tasks);

                return "Task Updated Successfully.";
            }
            catch (Exception exception)
            {
                return "Failed to Update Tasks";
            }
        }
    }
}
