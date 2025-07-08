using Agents.Build;
using System.Text;
using Todo.Core.Vacations;

namespace Agents.Graph
{
    public class OrchestrationDataNode(string name, IVacationPlanService vacationPlanService, IAgentProvider agentProvider) : INode
    {
        public async Task<NodeState> InvokeAsync(NodeState state)
        {
            var arguments = await GetArguments(state);

            state.AgentState.Arguments.Add("travel_task_list", arguments);

            return new NodeState(state.AgentState) { Source = Name, Headers = state.Headers, VacationPlanId = state.VacationPlanId,Route = AgentNames.OrchestratorAgent};
        }

        private async Task<string> GetArguments(NodeState state)
        {
            var vacationPlan = await vacationPlanService.GetAsync(state.VacationPlanId);

            var stringBuilder = new StringBuilder();

            foreach (var vacationPlanStage in vacationPlan.Stages)
            {
                stringBuilder.AppendLine($"[{vacationPlanStage.Stage} - '{vacationPlanStage.Description}' :{vacationPlanStage.Status}]");
            }

            return stringBuilder.ToString();
        }

        public string Name { get; } = name;
    }

}
