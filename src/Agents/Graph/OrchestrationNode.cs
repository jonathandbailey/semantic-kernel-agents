using Agents.Build;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using Microsoft.SemanticKernel;
using Todo.Core.Vacations;

namespace Agents.Graph
{
    public class OrchestrationNode(string name, IVacationPlanService vacationPlanService, IAgentProvider agentProvider) : INode
    {
        public async Task<NodeState> InvokeAsync(NodeState state)
        {
            var agent = await agentProvider.Create(Name);

            var request = AgentHeaderParser.RemoveHeaders(state.AgentState.Response.Content!);

            var requestState = new AgentState(Name)
            {
                Request = new ChatMessageContent(AuthorRole.User, request),
                Response = new ChatMessageContent(),
                Arguments = state.AgentState.Arguments,
                SessionId = state.AgentState.SessionId,
            };

            var arguments = await GetArguments(state);

            requestState.Arguments.Add("travel_task_list", arguments);

            var responseState = await agent.InvokeAsync(requestState);

            var headers = AgentHeaderParser.ExtractHeaders(responseState.Response.Content!);

            var stringBuilder = new StringBuilder();

            foreach (var header in headers)
            {
                stringBuilder.AppendLine(header);
            }

            return new NodeState(responseState) { Source = Name, Headers = stringBuilder.ToString(), VacationPlanId = state.VacationPlanId};
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
