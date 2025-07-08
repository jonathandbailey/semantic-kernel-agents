using System.Text;
using Todo.Core.Vacations;

namespace Agents.Middleware;

public class TravelDataMiddleware(IVacationPlanService vacationPlanService) : IAgentMiddleware
{
    public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
    {
        var vacationPlanId = state.Get<Guid>("VacationPlanId");
        
        var arguments = await GetArguments(vacationPlanId);

        state.Arguments["travel_task_list"] = arguments;

        return await next(state);
    }

    private async Task<string> GetArguments(Guid vacationPlanId)
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