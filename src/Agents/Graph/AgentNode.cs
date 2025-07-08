using Agents.Build;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace Agents.Graph
{
    public class AgentNode(string name, IAgentProvider agentProvider) : INode
    {
        public async Task<NodeState> InvokeAsync(NodeState state)
        {
            var agent = await agentProvider.Create(Name);
  
            var requestState = PreProcess(state);

            var responseState = await agent.InvokeAsync(requestState);

            return PostProcess(responseState, state);
        }

        private AgentState PreProcess(NodeState state)
        {
            var request = AgentHeaderParser.RemoveHeaders(state.AgentState.Response.Content!);

            var requestState = new AgentState(Name)
            {
                Request = new ChatMessageContent(AuthorRole.User, request),
                Response = new ChatMessageContent(),
                Arguments = AddHeadersToArguments(state.AgentState.Arguments, state.Headers),
                SessionId = state.AgentState.SessionId,
            };

            if (state.AgentState.HasKey("VacationPlanId"))
            {
                requestState.Set("VacationPlanId", state.AgentState.Get<Guid>("VacationPlanId"));
            }

            return requestState;
        }

        private NodeState PostProcess(AgentState agentState, NodeState state)
        {
            var headers = AgentHeaderParser.ExtractHeaders(agentState.Response.Content!);

            var stringBuilder = new StringBuilder();

            foreach (var header in headers)
            {
                stringBuilder.AppendLine(header);
            }

            var headerValues = AgentHeaderParser.ExtractHeaderValues(headers, "agent-invoke");

            var route = string.Empty;

            if (headerValues.ContainsKey("agent-invoke"))
            {
                route = headerValues["agent-invoke"];
            }

            return new NodeState(agentState) { Source = Name, Headers = stringBuilder.ToString(), VacationPlanId = state.VacationPlanId, Route = route};
        }

        private Dictionary<string, string> AddHeadersToArguments(Dictionary<string, string> arguments, string content)
        {
            var headers = AgentHeaderParser.ExtractHeaders(content);

            var headerValues = AgentHeaderParser.ExtractHeaderValues(headers, "task-id");

            if (headerValues.ContainsKey("task-id"))
            {
                arguments.Add("TaskId", headerValues["task-id"]);
            }

            return arguments;
        }

        public string Name { get; } = name;
    }
}
