using Todo.Core.Communication;

namespace Todo.Core.Agents.Middleware;

public class AgentDelegateWrapper(AgentDelegate agentDelegate) : IAgent
{
    public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request)
    {
        return await agentDelegate(request);
    }
}