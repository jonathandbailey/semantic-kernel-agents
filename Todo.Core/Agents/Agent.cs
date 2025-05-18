using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Communication;

namespace Todo.Core.Agents;

public class Agent(ChatCompletionAgent chatCompletionAgent, string name) : AgentBase, IAgent
{
    public string Name { get; } = name;

    public override async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request)
    {
        var stringBuilder = new StringBuilder();
  
        chatCompletionAgent.Kernel.Data.Add("sessionId", request.SessionId);

        await foreach (ChatMessageContent response in chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, request.Message), request.ChatHistory))
        {
            stringBuilder.Append(response.Content);
        }

        var message = stringBuilder.ToString();

        return new ChatCompletionResponse { Message = message, SessionId = request.SessionId, ChatHistory = request.ChatHistory};
    }
}