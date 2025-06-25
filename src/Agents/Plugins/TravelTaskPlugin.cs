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
    }
}
