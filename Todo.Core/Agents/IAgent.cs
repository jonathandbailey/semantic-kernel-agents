using Todo.Core.Communication;

namespace Todo.Core.Agents;

public interface IAgent
{
    Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request);
    string Name { get; }
}