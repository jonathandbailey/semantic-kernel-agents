using Agents.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;

namespace Agents.Tests;

public class GraphBuilderTests
{
    [Fact]
    public async Task GraphRun_WithAccommodationInvoke_RoutesToCorrectAgent()
    {
        const string userPrompt = "I want to plan a trip to Paris?";
        const string content = "User wants to plan Accommodation for their trip to Paris?";
        const string agentInvokeAccommodation = "[header-start]\n[agent-invoke:Accommodation]\n[header-end]\n" + content;
        const string accommodationAgentResponse = "Would you like to book a Hotel, Apartment or something else?";

        var graphBuilder = new GraphBuilder();

        var routingNode = new RoutingNode(NodeNames.Routing, NodeNames.Orchestration);

        var sessionId = Guid.NewGuid();

        var orchestrationAgentMoq =
            AgentTestHelper.CreateMockAgent(AgentNames.OrchestratorAgent, userPrompt, agentInvokeAccommodation, sessionId);

        var accommodationAgentMoq = AgentTestHelper.CreateMockAgent(AgentNames.AccommodationAgent, content,
            accommodationAgentResponse, sessionId);

        var expectedState = AgentTestHelper.CreateState(AgentNames.AccommodationAgent, content, sessionId);

        graphBuilder.AddNode(routingNode);
        graphBuilder.AddNode(NodeNames.Orchestration, orchestrationAgentMoq.Object);

        graphBuilder.AddNode(NodeNames.Accommodation, accommodationAgentMoq.Object);

        graphBuilder.Connect(NodeNames.Routing, NodeNames.Orchestration);
        graphBuilder.Connect(NodeNames.Orchestration, NodeNames.Accommodation);

        var userState = new AgentState(NodeNames.Routing) { Request = new ChatMessageContent(AuthorRole.User, userPrompt) };

        userState.Set("source", string.Empty);

        var graph = graphBuilder.Build();

        var responseState = await graph.RunAsync(NodeNames.Routing, new NodeState(userState));

        accommodationAgentMoq.Verify(x => x.InvokeAsync(It.Is<AgentState>(agentState => AgentTestHelper.Compare(agentState, expectedState))));

        Assert.Equal(sessionId, responseState.AgentState.SessionId);
    }
}