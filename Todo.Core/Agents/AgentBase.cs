using Todo.Core.Agents.Middleware;
using Todo.Core.Communication;

namespace Todo.Core.Agents
{
    public abstract class AgentBase : IAgentMiddleware
    {
        public abstract Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request);

        public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest context, AgentDelegate next)
        {
            return await InvokeAsync(context);
        }
    }
}
