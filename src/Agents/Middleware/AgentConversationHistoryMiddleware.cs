using Microsoft.SemanticKernel.Agents;
using Todo.Core.A2A;

namespace Agents.Middleware
{
    public  class AgentConversationHistoryMiddleware(IAgentChatHistoryProvider agentChatHistoryProvider, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
        {
            var agentTask = state.Get<AgentTask>("AgentTask");
            
            var chatHistory = await agentChatHistoryProvider.LoadChatHistoryAsync(agentName, agentTask.SessionId);

            AppendChatHistory(chatHistory, state);

            var response = await next(state);

            chatHistory = response.Get<ChatHistoryAgentThread>("ChatHistory");

            await agentChatHistoryProvider.SaveChatHistoryAsync(chatHistory, agentName, agentTask.SessionId);

            return response;
        }

        private void AppendChatHistory(ChatHistoryAgentThread chatHistory, AgentState agentState)
        {
            var stateChatHistory = agentState.HasKey("ChatHistory") ? agentState.Get<ChatHistoryAgentThread>("ChatHistory") : new ChatHistoryAgentThread();

            foreach (var message in chatHistory.ChatHistory)
            {
                stateChatHistory.ChatHistory.AddMessage(message.Role, message.Content!);
            }

            agentState.Set("ChatHistory", stateChatHistory);
        }
    }
}
