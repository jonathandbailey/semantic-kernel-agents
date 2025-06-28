using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Vacations;

namespace Agents.Graph
{
    public class TaskNode(string name, IVacationPlanService vacationPlanService) :INode
    {
        public async Task<NodeState> InvokeAsync(NodeState state)
        {
            var vacationPlanId = state.AgentState.Get<Guid>("vacationPlanId");

            var vacationPlan = await vacationPlanService.GetAsync(vacationPlanId);

            await vacationPlanService.UpdateAsync(vacationPlan, state.AgentState.AgentName);

            state.AgentState.Request = new ChatMessageContent(AuthorRole.Assistant, $"[header-start]\n[agent-invoke:User]\n[header-end]\n{state.AgentState.AgentName} has completed it's plan.");
            state.AgentState.Response = new ChatMessageContent(AuthorRole.Assistant, $"[header-start]\n[agent-invoke:User]\n[header-end]\n{state.AgentState.AgentName} has completed it's plan.");

            return state;
        }

        public string Name { get; } = name;
    }
}
