using Microsoft.SemanticKernel.Agents;

namespace Todo.Agents.Middleware
{
    public  class AgentConversationHistoryMiddleware(IAgentChatHistoryProvider agentChatHistoryProvider, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            var chatHistory = await agentChatHistoryProvider.LoadChatHistoryAsync(agentName, state.AgentTask.SessionId);

            state.Set("ChatHistory", chatHistory);

            var response = await next(state);

            chatHistory = response.Get<ChatHistoryAgentThread>("ChatHistory");

            await agentChatHistoryProvider.SaveChatHistoryAsync(chatHistory, agentName, state.AgentTask.SessionId);

            return response;
        }
    }
}
