using Microsoft.SemanticKernel.Agents;
using Todo.Core.A2A;

namespace Todo.Agents.Middleware
{
    public  class AgentConversationHistoryMiddleware(IAgentChatHistoryProvider agentChatHistoryProvider, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            var agentTask = state.Get<AgentTask>("AgentTask");
            
            var chatHistory = await agentChatHistoryProvider.LoadChatHistoryAsync(agentName, agentTask.SessionId);

            state.Set("ChatHistory", chatHistory);

            var response = await next(state);

            chatHistory = response.Get<ChatHistoryAgentThread>("ChatHistory");

            await agentChatHistoryProvider.SaveChatHistoryAsync(chatHistory, agentName, agentTask.SessionId);

            return response;
        }
    }
}
