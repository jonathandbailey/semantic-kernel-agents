using System.Text;
using Agents.Build;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agents.Graph
{
    public class AgentNode(string name, IAgentProvider agentProvider) : INode
    {
        public async Task<NodeState> InvokeAsync(NodeState state)
        {
            var agent = await agentProvider.Create(Name);

            var request = AgentHeaderParser.RemoveHeaders(state.AgentState.Response.Content!);

            var requestState = new AgentState(Name)
            {
                Request = new ChatMessageContent(AuthorRole.User, request),
                Response = new ChatMessageContent(),
                Arguments = AddHeadersToArguments(state.AgentState.Arguments, state.Headers),
                SessionId = state.AgentState.SessionId,
            };

            var responseState = await agent.InvokeAsync(requestState);

            var headers = AgentHeaderParser.ExtractHeaders(responseState.Response.Content!);

            var stringBuilder = new StringBuilder();

            foreach (var header in headers)
            {
                stringBuilder.AppendLine(header);
            }

            return new NodeState(responseState) { Source = Name, Headers = stringBuilder.ToString(), VacationPlanId = state.VacationPlanId};
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
