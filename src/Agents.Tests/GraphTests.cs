
using Agents.Build;
using Agents.Graph;
using Agents.Graph.v2;
using Microsoft.SemanticKernel;
using Moq;
using AgentGraph = Agents.Graph.v2.AgentGraph;
using AgentNode = Agents.Graph.v2.AgentNode;
using NodeNames = Agents.Graph.v2.NodeNames;

namespace Agents.Tests
{
    public class AgentGraphTests
    {
        [Fact]
        public async Task TestGraph()
        {
            const string userPrompt = "I want to plan a trip to Paris?";
            const string content = "User wants to plan Accommodation for their trip to Paris?";
            const string agentInvokeAccommodation = "[header-start]\n[agent-invoke:Accommodation]\n[header-end]\n" + content;
            const string accommodationAgentResponse = "Would you like to book a Hotel, Apartment or something else?";

            var sessionId = Guid.NewGuid();

            var agentProviderMoq = new Mock<IAgentProvider>();

            var orchestrationAgent = AgentTestHelper.CreateMockAgent(AgentNames.OrchestratorAgent, userPrompt,
                agentInvokeAccommodation, sessionId);
            
            agentProviderMoq.AddAgentToProvider(AgentNames.OrchestratorAgent, orchestrationAgent.Object);


            var registry = new Registry();
            
            var graph = new AgentGraph(registry);

    
            var orchestrationNode = new AgentNode("Orchestrator", Guid.NewGuid(), registry, agentProviderMoq.Object);
            var accommodationNode = new AgentNode("Accommodation", Guid.NewGuid(), registry, agentProviderMoq.Object);

            var routingNode = new RoutingNode("Routing", Guid.NewGuid(),  orchestrationNode.Id);

            graph.AddNode(routingNode);
            graph.AddNode(orchestrationNode);
            graph.AddNode(accommodationNode);

            graph.AddEdge(routingNode, orchestrationNode);
            graph.AddEdge(orchestrationNode, accommodationNode);

            var userState = new AgentState(NodeNames.Routing) { Request = new ChatMessageContent() };

            await graph.RunAsync(routingNode, new NodeState(userState));
        }
    }
}
