using Agents;
using Agents.Build;
using Agents.Graph;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Todo.Application.Services;

public class OrchestrationService(IAgentProvider agentProvider) : IOrchestrationService
{
    public async Task<NodeState> InvokeAsync(
        Guid sessionId,
        string message,
        Dictionary<string, string> arguments,
        string source,
        Guid vacationPlanId,
        Guid id)
    {
        var agentName = string.IsNullOrEmpty(source) ? AgentNames.UserAgent : source;

        var userState = CreateState(agentName, sessionId, message, arguments);

        userState.Set("VacationPlanId", vacationPlanId);
        userState.RequestId = id;

        var routingNode = new RoutingNode(NodeNames.Routing, NodeNames.Orchestration);
        var orchestrationAgent = await agentProvider.Create(AgentNames.OrchestratorAgent);

        var accommodationAgent = await agentProvider.Create(AgentNames.AccommodationAgent);
        var travelAgent = await agentProvider.Create(AgentNames.TravelAgent);

        var taskAgent = await agentProvider.Create(AgentNames.TaskAgent);
        
        var graphBuilder = new GraphBuilder();

        graphBuilder.AddNode(routingNode);
        
        graphBuilder.AddNode(NodeNames.Orchestration, orchestrationAgent);
        
        graphBuilder.AddNode(NodeNames.Accommodation, accommodationAgent);
        graphBuilder.AddNode(NodeNames.Travel, travelAgent);
        graphBuilder.AddNode(NodeNames.Task, taskAgent);

        graphBuilder.Connect(NodeNames.Routing, NodeNames.Orchestration);
        graphBuilder.Connect(NodeNames.Routing, NodeNames.Accommodation);
        graphBuilder.Connect(NodeNames.Routing, NodeNames.Travel);

        graphBuilder.Connect(NodeNames.Orchestration, NodeNames.Accommodation);
        graphBuilder.Connect(NodeNames.Orchestration, NodeNames.Travel);

        graphBuilder.Connect(NodeNames.Accommodation, NodeNames.Task);
        graphBuilder.Connect(NodeNames.Travel, NodeNames.Task);
        
        graphBuilder.Connect(NodeNames.Task, NodeNames.Orchestration);

        var graph = graphBuilder.Build();

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
    Task<NodeState> InvokeAsync(Guid sessionId, string message, Dictionary<string,string> arguments, string source, Guid vacationPlanId, Guid id);
}