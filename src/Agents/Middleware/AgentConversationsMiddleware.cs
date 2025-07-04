using Microsoft.SemanticKernel.Agents;

namespace Agents.Middleware
{
    public class AgentConversationsMiddleware(IAgentChatHistoryProvider agentChatHistoryProvider) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            var response = await next(state);

            var chatHistory = response.Get<ChatHistoryAgentThread>("ChatHistory");

            await agentChatHistoryProvider.AddToConversation(state.SessionId.ToString(), chatHistory, state.AgentName);

            return response;
        }
    }
}
