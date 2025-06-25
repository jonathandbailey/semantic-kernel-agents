using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Agents;
using Agents.Build;
using Agents.Graph;
using Todo.Core.Vacations;

namespace Todo.Application.Services;

public class OrchestrationService(IAgentProvider agentProvider, IVacationPlanService vacationPlanService) : IOrchestrationService
{
    public async Task<AgentState> InvokeAsync(
        string sessionId,
        string message,
        Dictionary<string, string> arguments,
        Func<StreamingChatMessageContent, string, bool, Task> streamingMessageCallback,
        string source,
        Guid vacationPlanId)
    {
        var agentName = string.IsNullOrEmpty(source) ? AgentNames.UserAgent : source;

        var userState = CreateState(agentName, sessionId, message, arguments);

        userState.Set("source", source);
        userState.Set("vacationPlanId", vacationPlanId);

        var graph = new AgentGraph();

        graph.AddNode(new HeaderRoutingNode("Root", AgentNames.UserAgent));
        graph.AddNode(new AgentNode(AgentNames.UserAgent, agentProvider, streamingMessageCallback));
        graph.AddNode(new AgentNode(AgentNames.AccommodationAgent, agentProvider, streamingMessageCallback));
        
        graph.Connect("Root", AgentNames.UserAgent);
        graph.Connect("Root", AgentNames.AccommodationAgent);

        graph.Connect(AgentNames.UserAgent, AgentNames.AccommodationAgent);

        graph.AddNode(new TaskNode("Task", vacationPlanService));

        graph.Connect(AgentNames.AccommodationAgent, new AgentTaskCompleteEdge("Task"));


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

        state.SetSessionId(!string.IsNullOrWhiteSpace(sessionId) ? sessionId : Guid.NewGuid().ToString());

        return state;
    }
}



public interface IOrchestrationService
{
    Task<AgentState> InvokeAsync(string sessionId, string message, Dictionary<string,string> arguments, Func<StreamingChatMessageContent, string, bool, Task> streamingMessageCallback, string source, Guid vacationPlanId);
}