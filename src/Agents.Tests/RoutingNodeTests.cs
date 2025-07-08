using Agents.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;

namespace Agents.Tests
{
    public class RoutingNodeTests
    {
        private const string routingTemplate = "[agent-invoke:{0}]";

        [Fact]
        public async Task RoutingNodeRun_WithSourceSet_RoutesToCorrectAgent()
        {
            const string routingNodeName = "Routing";
            
            var graph = new AgentGraph();

            graph.AddNode(new RoutingNode(routingNodeName, AgentNames.UserAgent));

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };
        
            var finalState = await graph.RunAsync(routingNodeName, new NodeState(userState) {Source = AgentNames.AccommodationAgent});

            var hasRoutingHeader = AgentHeaderParser.HasHeader(string.Format(routingTemplate, AgentNames.AccommodationAgent), finalState.Headers);
            
            Assert.True(hasRoutingHeader, "Response does not contain the correct routing header.");
        }

        [Fact]
        public async Task RoutingNodeRun_WithSourceNotSet_RoutesToCorrectAgent()
        {
            const string routingNodeName = "Routing";

            var graph = new AgentGraph();

            graph.AddNode(new RoutingNode(routingNodeName, AgentNames.UserAgent));

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", string.Empty);

            var finalState = await graph.RunAsync(routingNodeName, new NodeState(userState));

            var hasRoutingHeader = AgentHeaderParser.HasHeader(string.Format(routingTemplate, AgentNames.UserAgent), finalState.Headers);

            Assert.True(hasRoutingHeader, "Response does not contain the correct routing header.");
        }

        [Fact]
        public async Task RoutingNodeRun_WithEdge_CallsCorrectUserAgent()
        {
            const string routingNodeName = "Routing";

            var moqUserNode = new Mock<INode>();

            moqUserNode.Setup(x => x.Name).Returns("User");

            var userAgentState = new AgentState(AgentNames.UserAgent)
            {
                Request = new ChatMessageContent(),
                Response = new ChatMessageContent(AuthorRole.Assistant, string.Format(routingTemplate, AgentNames.AccommodationAgent))
            };

            moqUserNode.Setup(x => x.InvokeAsync(It.IsAny<NodeState>())).ReturnsAsync(() => new NodeState(userAgentState));

            var graph = new AgentGraph();

            graph.AddNode(new RoutingNode(routingNodeName, AgentNames.UserAgent));

            graph.AddNode(moqUserNode.Object);

            graph.SetEdges(routingNodeName, [new AgentInvokeEdge(AgentNames.UserAgent)]);

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", string.Empty);

            await graph.RunAsync(routingNodeName, new NodeState(userState));
         
            moqUserNode.Verify( x=> x.InvokeAsync(It.IsAny<NodeState>()));
        }

        [Fact]
        public async Task RoutingNodeRun_WithEdge_CallsCorrectAccommodationAgent()
        {
            const string routingNodeName = "Routing";

            var moqUserNode = new Mock<INode>();
            var moqAccommodationNode = new Mock<INode>();

            moqUserNode.Setup(x => x.Name).Returns("User");
            moqAccommodationNode.Setup(x => x.Name).Returns("Accommodation");

            var accommodationAgentState = new AgentState(AgentNames.AccommodationAgent)
            {
                Request = new ChatMessageContent(),
                Response = new ChatMessageContent()
            };

            moqAccommodationNode.Setup(x => x.InvokeAsync(It.IsAny<NodeState>()))
                .ReturnsAsync(new NodeState(accommodationAgentState));

            var graph = new AgentGraph();

            graph.AddNode(new RoutingNode(routingNodeName, AgentNames.UserAgent));

            graph.AddNode(moqUserNode.Object);
            graph.AddNode(moqAccommodationNode.Object);

            graph.SetEdges(routingNodeName, 
                [new AgentInvokeEdge(AgentNames.UserAgent), 
                    new AgentInvokeEdge(AgentNames.AccommodationAgent)]);

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };
     
            await graph.RunAsync(routingNodeName, new NodeState(userState) {Source = AgentNames.AccommodationAgent});

            moqAccommodationNode.Verify(x => x.InvokeAsync(It.IsAny<NodeState>()));
        }
    }
}
