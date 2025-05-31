using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Communication;

namespace Todo.Core.Agents;

public class Agent(ChatCompletionAgent chatCompletionAgent, string name) : AgentBase, IAgent
{
    public string Name { get; } = name;

    public override async Task<AgentState> InvokeAsync(AgentState request)
    {
        var stringBuilder = new StringBuilder();
 
        await foreach (ChatMessageContent response in chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, request.ChatCompletionRequest.Message), request.ChatCompletionRequest.ChatHistory))
        {
            stringBuilder.Append(response.Content);
        }

        var message = stringBuilder.ToString();

        request.ChatCompletionResponse = new ChatCompletionResponse { Message = message, SessionId = request.SessionId, ChatHistory = request.ChatCompletionRequest.ChatHistory};

        return request;
    }
}