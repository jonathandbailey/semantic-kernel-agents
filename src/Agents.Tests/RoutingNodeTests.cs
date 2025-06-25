using Agents.Graph;
using Microsoft.SemanticKernel;
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

            graph.AddNode(new HeaderRoutingNode(routingNodeName, AgentNames.UserAgent));

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", AgentNames.AccommodationAgent);

            var finalState = await graph.RunAsync(routingNodeName, new NodeState(userState));

            var hasRoutingHeader = AgentHeaderParser.HasHeader(string.Format(routingTemplate, AgentNames.AccommodationAgent), finalState.AgentState.Response.Content!);
            
            Assert.True(hasRoutingHeader, "Response does not contain the correct routing header.");
        }

        [Fact]
        public async Task RoutingNodeRun_WithSourceNotSet_RoutesToCorrectAgent()
        {
            const string routingNodeName = "Routing";

            var graph = new AgentGraph();

            graph.AddNode(new HeaderRoutingNode(routingNodeName, AgentNames.UserAgent));

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", string.Empty);

            var finalState = await graph.RunAsync(routingNodeName, new NodeState(userState));

            var hasRoutingHeader = AgentHeaderParser.HasHeader(string.Format(routingTemplate, AgentNames.UserAgent), finalState.AgentState.Response.Content!);

            Assert.True(hasRoutingHeader, "Response does not contain the correct routing header.");
        }

        [Fact]
        public async Task RoutingNodeRun_WithEdge_CallsCorrectUserAgent()
        {
            const string routingNodeName = "Routing";

            var moqUserNode = new Mock<INode>();

            moqUserNode.Setup(x => x.Name).Returns("User");

            var graph = new AgentGraph();

            graph.AddNode(new HeaderRoutingNode(routingNodeName, AgentNames.UserAgent));

            graph.AddNode(moqUserNode.Object);

            graph.SetEdges(routingNodeName, [new AgentInvokeEdge(AgentNames.UserAgent)]);

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", string.Empty);

            await graph.RunAsync(routingNodeName, new NodeState(userState));
         
            moqUserNode.Verify( x=> x.InvokeAsync(It.IsAny<AgentState>()));
        }

        [Fact]
        public async Task RoutingNodeRun_WithEdge_CallsCorrectAccommodationAgent()
        {
            const string routingNodeName = "Routing";

            var moqUserNode = new Mock<INode>();
            var moqAccommodationNode = new Mock<INode>();

            moqUserNode.Setup(x => x.Name).Returns("User");
            moqAccommodationNode.Setup(x => x.Name).Returns("Accommodation");

            var graph = new AgentGraph();

            graph.AddNode(new HeaderRoutingNode(routingNodeName, AgentNames.UserAgent));

            graph.AddNode(moqUserNode.Object);
            graph.AddNode(moqAccommodationNode.Object);

            graph.SetEdges(routingNodeName, 
                [new AgentInvokeEdge(AgentNames.UserAgent), 
                    new AgentInvokeEdge(AgentNames.AccommodationAgent)]);

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", AgentNames.AccommodationAgent);

            await graph.RunAsync(routingNodeName, new NodeState(userState));

            moqAccommodationNode.Verify(x => x.InvokeAsync(It.IsAny<AgentState>()));
        }
    }
}
