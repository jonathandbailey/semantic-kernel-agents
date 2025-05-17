using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Communication;

namespace Todo.Core.Agents;

#pragma warning disable SKEXP0110

public class Agent : AgentBase, IAgent
{
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public string Name { get; }

    public Agent(ChatCompletionAgent chatCompletionAgent, string name)
    {
        Name = name;

        _chatCompletionAgent = chatCompletionAgent;
    }

    public override async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request)
    {
        var stringBuilder = new StringBuilder();
  
        _chatCompletionAgent.Kernel.Data.Add("sessionId", request.SessionId);

        await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, request.Message), request.ChatHistory))
        {
            stringBuilder.Append(response.Content);
        }

        var message = stringBuilder.ToString();

        return new ChatCompletionResponse { Message = message, SessionId = request.SessionId, ChatHistory = request.ChatHistory};
    }
}