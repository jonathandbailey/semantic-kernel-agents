using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Agents;
using Agents.Build;
using Agents.Graph;
using Todo.Core.Vacations;

namespace Todo.Application.Services;

public class OrchestrationService(IAgentProvider agentProvider, IVacationPlanService vacationPlanService) : IOrchestrationService
{
    public async Task<NodeState> InvokeAsync(
        Guid sessionId,
        string message,
        Dictionary<string, string> arguments,
        string source,
        Guid vacationPlanId)
    {
        var agentName = string.IsNullOrEmpty(source) ? AgentNames.UserAgent : source;

        var userState = CreateState(agentName, sessionId, message, arguments);

        userState.Set("source", source);
        userState.Set("vacationPlanId", vacationPlanId);

        var graph = new AgentGraph();

        graph.AddNode(new HeaderRoutingNode("Root", AgentNames.UserAgent));
        graph.AddNode(new AgentNode(AgentNames.UserAgent, agentProvider));
        graph.AddNode(new AgentNode(AgentNames.AccommodationAgent, agentProvider));
        
        graph.Connect("Root", AgentNames.UserAgent);
        graph.Connect("Root", AgentNames.AccommodationAgent);

        graph.Connect(AgentNames.UserAgent, AgentNames.AccommodationAgent);

        graph.AddNode(new TaskNode("Task", vacationPlanService));

        graph.Connect(AgentNames.AccommodationAgent, new AgentTaskCompleteEdge("Task"));

        graph.Connect("Task", AgentNames.UserAgent);


        var finalState = await graph.RunAsync("Root", new NodeState(userState) { Source = source });

        return finalState;
    }

    private static AgentState CreateState(string agentName, Guid sessionId, string message,
        Dictionary<string, string> arguments)
    {
        var state = new AgentState(agentName)
        {
            Request = new ChatMessageContent(AuthorRole.User, message),
            Arguments = arguments
        };

        state.SessionId = sessionId == Guid.Empty ? Guid.NewGuid() : sessionId;

        return state;
    }
}



public interface IOrchestrationService
{
    Task<NodeState> InvokeAsync(Guid sessionId, string message, Dictionary<string,string> arguments, string source, Guid vacationPlanId);
}