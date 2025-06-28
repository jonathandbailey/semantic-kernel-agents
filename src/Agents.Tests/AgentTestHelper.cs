using Agents.Build;
using Agents.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;

namespace Agents.Tests
{
    public static class AgentTestHelper
    {
        public const string RoutingNodeName = "Routing";

        public static Mock<IAgent> CreateMockAgent(string agentName, string request, string response, Guid sessionId)
        {
            var agentMoq = new Mock<IAgent>();

            var agentResponseState = new AgentState(agentName)
            {
                Request = new ChatMessageContent(AuthorRole.User, request),
                Response = new ChatMessageContent(AuthorRole.Assistant, response),
                SessionId = sessionId
            };

            agentMoq.Setup(x => x.InvokeAsync(It.IsAny<AgentState>())).ReturnsAsync(agentResponseState);
            agentMoq.Setup(x => x.Name).Returns(agentName);

            return agentMoq;
        }

        public static Mock<IAgent> CreateMockAgent(string agentName, AgentState agentResponseState)
        {
            var agentMoq = new Mock<IAgent>();

            agentMoq.Setup(x => x.InvokeAsync(It.IsAny<AgentState>())).ReturnsAsync(agentResponseState);
            agentMoq.Setup(x => x.Name).Returns(agentName);

            return agentMoq;
        }

        public static void AddAgentToProvider(this Mock<IAgentProvider> provider, string agentName, IAgent agent)
        {
            provider.Setup(x => x.Create(agentName)).ReturnsAsync(agent);
        }

        public static bool Compare(AgentState source, AgentState target)
        {
            return source.Response.Content == target.Response.Content
                && source.Response.Content == target.Response.Content
                && source.SessionId == target.SessionId;
        }

        public static AgentState CreateState(string agentName, string requestContent)
        {
            return new AgentState(agentName)
            {
                Request = new ChatMessageContent { Role = AuthorRole.User, Content = requestContent },
                Response = new ChatMessageContent()
            };
        }

        public static AgentState CreateState(string agentName, string requestContent, Guid sessionId)
        {
            return new AgentState(agentName)
            {
                Request = new ChatMessageContent { Role = AuthorRole.User, Content = requestContent },
                Response = new ChatMessageContent(),
                SessionId = sessionId
            };
        }

        public static AgentGraph BuildGraph(IMock<IAgent> userAgentMoq, IMock<IAgent> accommodationAgentMoq)
        {
            var agentProviderMoq = new Mock<IAgentProvider>();
        
            agentProviderMoq.AddAgentToProvider(AgentNames.UserAgent, userAgentMoq.Object);
            agentProviderMoq.AddAgentToProvider(AgentNames.AccommodationAgent, accommodationAgentMoq.Object);

            var userAgentNode = new AgentNode(AgentNames.UserAgent, agentProviderMoq.Object);
            var accommodationNode = new AgentNode(AgentNames.AccommodationAgent, agentProviderMoq.Object);

            var graph = new AgentGraph();

            graph.AddNode(new HeaderRoutingNode(RoutingNodeName, AgentNames.UserAgent));
            graph.AddNode(userAgentNode);
            graph.AddNode(accommodationNode);

            graph.Connect(RoutingNodeName, AgentNames.UserAgent).
                Connect(AgentNames.UserAgent, AgentNames.AccommodationAgent);

            return graph;
        }
    }
}
