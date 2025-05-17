using Todo.Core.Communication;

namespace Todo.Core.Agents.Middleware
{
    public  class AgentConversationHistoryMiddleware(IAgentChatHistoryProvider agentChatHistoryProvider, string agentName) : IAgentMiddleware
    {
        public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest context, AgentDelegate next)
        {
            context.ChatHistory = await agentChatHistoryProvider.LoadChatHistoryAsync($"{context.SessionId} - [{agentName}]");

            var response = await next(context);

            await agentChatHistoryProvider.SaveChatHistoryAsync(response.ChatHistory,
                $"{context.SessionId} - [{agentName}]");

            return response;
        }
    }
}
