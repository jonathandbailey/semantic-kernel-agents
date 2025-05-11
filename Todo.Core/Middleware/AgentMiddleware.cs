using Todo.Core.Agents;
using Todo.Core.Communication;

namespace Todo.Core.Middleware
{
    public class AgentMiddleware(IAgent agent) : IAgentMiddleware
    {
        public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest context, AgentDelegate next)
        {
            return await agent.InvokeAsync(context);
        }
    }
}
