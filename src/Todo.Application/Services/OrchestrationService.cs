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

        userState.Set("VacationPlanId", vacationPlanId);


        var routingNode = new HeaderRoutingNode(NodeNames.Routing, AgentNames.OrchestratorAgent);

        var graph = new AgentGraph();

        graph.AddNode(routingNode);
        
        graph.AddNode(new AgentNode(AgentNames.OrchestratorAgent, agentProvider));
        
        graph.AddNode(new AgentNode(AgentNames.AccommodationAgent, agentProvider));
        graph.AddNode(new AgentNode(AgentNames.TravelAgent, agentProvider));
        graph.AddNode(new AgentNode(AgentNames.TaskAgent, agentProvider));

        graph.Connect(NodeNames.Routing, AgentNames.OrchestratorAgent);
        graph.Connect(NodeNames.Routing, AgentNames.AccommodationAgent);
        graph.Connect(NodeNames.Routing, AgentNames.TravelAgent);

        graph.Connect(AgentNames.OrchestratorAgent, AgentNames.AccommodationAgent);
        graph.Connect(AgentNames.OrchestratorAgent, AgentNames.TravelAgent);

        graph.Connect(AgentNames.AccommodationAgent, AgentNames.TaskAgent);
        graph.Connect(AgentNames.TravelAgent, AgentNames.TaskAgent);
        
        graph.Connect(AgentNames.TaskAgent, AgentNames.OrchestratorAgent);

        var finalState = await graph.RunAsync(NodeNames.Routing, new NodeState(userState) { Source = source, VacationPlanId = vacationPlanId});

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