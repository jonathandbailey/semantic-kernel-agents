namespace Todo.Agents.Middleware
{
    public  class AgentConversationHistoryMiddleware(IAgentChatHistoryProvider agentChatHistoryProvider, string agentName) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
        {
            context.ChatCompletionRequest.ChatHistory = await agentChatHistoryProvider.LoadChatHistoryAsync(agentName, context.SessionId);

            var response = await next(context);

            await agentChatHistoryProvider.SaveChatHistoryAsync(response.ChatCompletionResponse!.ChatHistory, agentName, context.SessionId);

            return response;
        }
    }
}
