using Todo.Core.Communication;

namespace Todo.Core.Agents;

public interface IAgent
{
    public Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request);
    string Name { get; }
}