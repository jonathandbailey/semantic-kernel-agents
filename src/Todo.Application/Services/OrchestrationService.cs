using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Agents;
using Agents.Build;
using Agents.Graph;

namespace Todo.Application.Services;

public class OrchestrationService(IAgentProvider agentProvider) : IOrchestrationService
{
    public async Task<AgentState> InvokeAsync(
        string sessionId,
        string message,
        Dictionary<string, string> arguments,
        Func<StreamingChatMessageContent, string, bool, Task> streamingMessageCallback,
        string source)
    {
        var agentName = string.IsNullOrEmpty(source) ? AgentNames.UserAgent : source;

        var userState = CreateState(agentName, sessionId, message, arguments);

        userState.Set("source", source);

        var graph = new AgentGraph();

        graph.AddNode(new RootNode("Root"));
        graph.AddNode(new AgentNode(AgentNames.UserAgent, agentProvider, streamingMessageCallback));
        graph.AddNode(new AgentNode(AgentNames.AccommodationAgent, agentProvider, streamingMessageCallback));
        
        graph.AddEdge("Root", [new AgentInvokeEdge(AgentNames.UserAgent), new AgentInvokeEdge(AgentNames.AccommodationAgent)]);

        graph.AddEdge(AgentNames.UserAgent, [ new AgentInvokeEdge(AgentNames.AccommodationAgent)]);

        var finalState = await graph.RunAsync("Root", new NodeState(userState));

        return finalState.AgentState;
    }

    private static AgentState CreateState(string agentName, string sessionId, string message,
        Dictionary<string, string> arguments)
    {
        var state = new AgentState(agentName)
        {
            Request = new ChatMessageContent(AuthorRole.User, message),
            Arguments = arguments
        };

        state.SetTaskId(Guid.NewGuid().ToString());

        state.SetSessionId(!string.IsNullOrWhiteSpace(sessionId) ? sessionId : Guid.NewGuid().ToString());

        return state;
    }
}



public interface IOrchestrationService
{
    Task<AgentState> InvokeAsync(string sessionId, string message, Dictionary<string,string> arguments, Func<StreamingChatMessageContent, string, bool, Task> streamingMessageCallback, string source);
}