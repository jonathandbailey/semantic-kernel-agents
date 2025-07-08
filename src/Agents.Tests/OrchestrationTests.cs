using Agents.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;

namespace Agents.Tests
{
    public class OrchestrationTests
    {
        private const string routingTemplate = "[header-start]\n[agent-invoke:{0}]\n[header-end]";

        [Fact]
        public async Task RunGraph_WithAgentToAgentInvoke_CallsCorrectAgent()
        {
            const string routingNodeName = NodeNames.Routing;

            var moqOrchestrationNode = new Mock<INode>();
            moqOrchestrationNode.Setup(x => x.Name).Returns(NodeNames.Orchestration);
            var orchestrationAgentState = new AgentState(AgentNames.UserAgent)
            {
                Request = new ChatMessageContent(),
                Response = new ChatMessageContent(AuthorRole.Assistant, string.Format(routingTemplate, AgentNames.AccommodationAgent))
            };

            var userNodeState = new NodeState(orchestrationAgentState)
                { Headers = string.Format(routingTemplate, AgentNames.AccommodationAgent), Route = NodeNames.Accommodation};

            moqOrchestrationNode.Setup(x => x.InvokeAsync(It.IsAny<NodeState>())).ReturnsAsync(() => userNodeState);

            var moqAccommodationNode = new Mock<INode>();
            
            moqAccommodationNode.Setup(x => x.Name).Returns(NodeNames.Accommodation);

            var accommodationAgentState = new AgentState(AgentNames.AccommodationAgent)
            {
                Request = new ChatMessageContent(),
                Response = new ChatMessageContent()
            };

            moqAccommodationNode.Setup(x => x.InvokeAsync(It.IsAny<NodeState>()))
                .ReturnsAsync(new NodeState(accommodationAgentState));

            var graphBuilder = new GraphBuilder();

            graphBuilder.AddNode(new RoutingNode(routingNodeName, NodeNames.Orchestration));

            graphBuilder.AddNode(moqOrchestrationNode.Object);
            graphBuilder.AddNode(moqAccommodationNode.Object);
      
            graphBuilder.Connect(NodeNames.Routing, NodeNames.Orchestration);
            graphBuilder.Connect(NodeNames.Routing, NodeNames.Accommodation);

            graphBuilder.Connect(NodeNames.Orchestration, NodeNames.Accommodation);

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", AgentNames.OrchestratorAgent);

            var graph = graphBuilder.Build();

            await graph.RunAsync(routingNodeName, new NodeState(userState));

            moqAccommodationNode.Verify(x => x.InvokeAsync(It.IsAny<NodeState>()));
            moqOrchestrationNode.Verify(x => x.InvokeAsync(It.IsAny<NodeState>()));
        }
    }
}
