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

            var requestState = new AgentState(Name)
            {
                Request = new ChatMessageContent(AuthorRole.User, AgentHeaderParser.RemoveHeaders(state.AgentState.Response.Content!)),
                Response = new ChatMessageContent(),
                Arguments = state.AgentState.Arguments,
                SessionId = state.AgentState.SessionId,
            };

            var responseState = await agent.InvokeAsync(requestState);

            var headers = AgentHeaderParser.ExtractHeaders(responseState.Response.Content!);

            var stringBuilder = new StringBuilder();

            foreach (var header in headers)
            {
                stringBuilder.AppendLine(header);
            }

            return new NodeState(responseState) { Source = Name, Headers = stringBuilder.ToString()};
        }

        public string Name { get; } = name;
    }
}
