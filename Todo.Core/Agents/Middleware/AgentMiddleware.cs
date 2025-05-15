using Todo.Core.Communication;

namespace Todo.Core.Agents.Middleware;

public class AgentMiddleware(IAgent agent) : IAgentMiddleware
{
    public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest context, AgentDelegate next)
    {
        return await agent.InvokeAsync(context);
    }
}