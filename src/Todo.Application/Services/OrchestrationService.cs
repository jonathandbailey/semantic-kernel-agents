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

        const string routingNodeName = "Routing";

        var graph = new AgentGraph();

        graph.AddNode(new HeaderRoutingNode(routingNodeName, AgentNames.UserAgent));
        graph.AddNode(new OrchestrationNode(AgentNames.UserAgent, vacationPlanService, agentProvider));
        graph.AddNode(new AgentNode(AgentNames.AccommodationAgent, agentProvider));
        graph.AddNode(new AgentNode(AgentNames.TravelAgent, agentProvider));
        graph.AddNode(new AgentNode(AgentNames.TaskAgent, agentProvider));

        graph.Connect(routingNodeName, AgentNames.UserAgent);
        graph.Connect(routingNodeName, AgentNames.AccommodationAgent);
        graph.Connect(routingNodeName, AgentNames.TravelAgent);

        graph.Connect(AgentNames.UserAgent, AgentNames.AccommodationAgent);
        graph.Connect(AgentNames.UserAgent, AgentNames.TravelAgent);

        graph.Connect(AgentNames.AccommodationAgent, AgentNames.TaskAgent);
        graph.Connect(AgentNames.TravelAgent, AgentNames.TaskAgent);
        
        graph.Connect(AgentNames.TaskAgent, AgentNames.UserAgent);

        var finalState = await graph.RunAsync(routingNodeName, new NodeState(userState) { Source = source, VacationPlanId = vacationPlanId});

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