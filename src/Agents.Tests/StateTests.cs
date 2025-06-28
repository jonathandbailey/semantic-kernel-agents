using Agents.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;

namespace Agents.Tests;

public class StateTests
{
    [Fact]
    public async Task HeaderRoutingNode_WithNoSource_ReturnsCorrectStateAndRouting()
    {
        const string routingNodeName = "Routing";
        const string userPrompt = "I want to plan a trip to Paris?";
        const string expectedHeaders = "[header-start]\n[agent-invoke:User]\n[header-end]";

        var graph = new AgentGraph();

        graph.AddNode(new HeaderRoutingNode(routingNodeName, AgentNames.UserAgent));

        var routingState = new AgentState(routingNodeName) { Request = new ChatMessageContent() { Role = AuthorRole.User, Content = userPrompt} };

        var nodeState = new NodeState(routingState);

        var responseState =  await graph.RunAsync(routingNodeName, nodeState);

        Assert.Equal(AgentNames.UserAgent, responseState.Source);

        Assert.Equal(expectedHeaders, responseState.Headers);

        Assert.Equal(userPrompt, responseState.AgentState.Response.Content);
    }

    [Fact]
    public async Task UserAgent_WithHeaderRouting_InvokesAccommodationAgentWithCorrectState()
    {
        const string userPrompt = "I want to plan a trip to Paris?";
        const string content = "User wants to plan Accommodation for their trip to Paris?";
        const string agentInvokeAccommodation = "[header-start]\n[agent-invoke:Accommodation]\n[header-end]\n" + content;
        const string accommodationAgentResponse = "Would you like to book a Hotel, Apartment or something else?";

        var sessionId = Guid.NewGuid();

        var userAgentMoq = AgentTestHelper.CreateMockAgent(AgentNames.UserAgent, userPrompt, agentInvokeAccommodation, sessionId);
        var accommodationAgentMoq = AgentTestHelper.CreateMockAgent(AgentNames.AccommodationAgent, content, accommodationAgentResponse, sessionId);

        var graph = AgentTestHelper.BuildGraph(userAgentMoq, accommodationAgentMoq);

        var initialState = AgentTestHelper.CreateState(AgentTestHelper.RoutingNodeName, userPrompt, sessionId);
        var expectedState = AgentTestHelper.CreateState(AgentNames.AccommodationAgent, content, sessionId);

        var nodeState = new NodeState(initialState);

        var responseState =  await graph.RunAsync(AgentTestHelper.RoutingNodeName, nodeState);
        
        userAgentMoq.Verify(x => x.InvokeAsync(It.IsAny<AgentState>()));
        accommodationAgentMoq.Verify(x => x.InvokeAsync(It.Is<AgentState>( agentState => AgentTestHelper.Compare(agentState, expectedState))));

        Assert.Equal(AgentNames.AccommodationAgent, responseState.Source);
        Assert.Equal(accommodationAgentResponse, responseState.AgentState.Response.Content);

        Assert.Equal(sessionId, responseState.AgentState.SessionId);
    }
}