using Todo.Core.Vacations;

namespace Agents.Graph
{
    public class TaskNode(string name, IVacationPlanService vacationPlanService) :INode
    {
        public async Task<AgentState> InvokeAsync(AgentState state)
        {
            var vacationPlanId = state.Get<Guid>("vacationPlanId");

            var vacationPlan = await vacationPlanService.GetAsync(vacationPlanId);

            await vacationPlanService.UpdateAsync(vacationPlan, state.AgentName);
            
            return state;
        }

        public string Name { get; } = name;
    }
}
