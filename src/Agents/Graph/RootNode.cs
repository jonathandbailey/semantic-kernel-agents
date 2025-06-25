using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agents.Graph
{
    public class HeaderRoutingNode(string name, string defaultNodeName) : INode
    {
        private const string routingTemplate = "[header-start]\n[agent-invoke:{0}]\n[header-end]";
        
        public Task<AgentState> InvokeAsync(AgentState state)
        {
            var source = state.Get<string>("source");

            if (string.IsNullOrEmpty(source))
            {
                state.Set("source", defaultNodeName);

                state.Response = new ChatMessageContent(AuthorRole.Assistant,
                    string.Format(routingTemplate, AgentNames.UserAgent));
            }
            else
            {
                state.Response = new ChatMessageContent(AuthorRole.Assistant,
                    string.Format(routingTemplate, source));
            }
            
            return Task.FromResult(state);
        }

        public string Name { get; } = name; 
    }
}
