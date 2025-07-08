using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agents.Graph;

public class RoutingNode(string name, string defaultNodeName) : INode
{
    private const string routingTemplate = "[header-start]\n[agent-invoke:{0}]\n[header-end]";
        
    public Task<NodeState> InvokeAsync(NodeState state)
    {
        state.Source = string.IsNullOrEmpty(state.Source) ? defaultNodeName : state.Source;
           
        state.Headers = string.Format(routingTemplate, state.Source);

        state.AgentState.Response = new ChatMessageContent(AuthorRole.Assistant, state.AgentState.Request.Content);

        state.Route = state.Source;

        return Task.FromResult(state);
    }

    public string Name { get; } = name;
    public Guid Id { get; } = Guid.NewGuid();
}