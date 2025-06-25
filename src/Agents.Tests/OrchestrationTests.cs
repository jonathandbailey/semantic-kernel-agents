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
            const string routingNodeName = "Routing";

            var moqUserNode = new Mock<INode>();
            var moqAccommodationNode = new Mock<INode>();

            moqUserNode.Setup(x => x.Name).Returns("User");
            moqAccommodationNode.Setup(x => x.Name).Returns("Accommodation");

            var userAgentState = new AgentState(AgentNames.UserAgent)
            {
                Request = new ChatMessageContent(),
                Response = new ChatMessageContent(AuthorRole.Assistant, string.Format(routingTemplate, AgentNames.AccommodationAgent))
            };

            moqUserNode.Setup(x => x.InvokeAsync(It.IsAny<AgentState>())).ReturnsAsync(() => userAgentState);

            var graph = new AgentGraph();

            graph.AddNode(new HeaderRoutingNode(routingNodeName, AgentNames.UserAgent));

            graph.AddNode(moqUserNode.Object);
            graph.AddNode(moqAccommodationNode.Object);
      
            graph.Connect(routingNodeName, AgentNames.UserAgent);
            graph.Connect(routingNodeName, AgentNames.AccommodationAgent);

            graph.Connect(AgentNames.UserAgent, AgentNames.AccommodationAgent);

            var userState = new AgentState(routingNodeName) { Request = new ChatMessageContent() };

            userState.Set("source", AgentNames.UserAgent);

            await graph.RunAsync(routingNodeName, new NodeState(userState));

            moqAccommodationNode.Verify(x => x.InvokeAsync(It.IsAny<AgentState>()));
            moqUserNode.Verify(x => x.InvokeAsync(It.IsAny<AgentState>()));
        }
    }
}
