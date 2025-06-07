using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using System.Text;

namespace Todo.Agents;

public class Agent(ChatCompletionAgent chatCompletionAgent, string name) : AgentBase, IAgent
{
    public string Name { get; } = name;

    public override async Task<AgentState> InvokeAsync(AgentState state)
    {
        var stringBuilder = new StringBuilder();

        var chatHistory = state.Get<ChatHistoryAgentThread>("ChatHistory");

        await foreach (ChatMessageContent response in chatCompletionAgent.InvokeAsync(state.Request, chatHistory))
        {
            stringBuilder.Append(response.Content);

            state.Responses.Add(response);
        }

        var message = stringBuilder.ToString();

        state.ChatCompletionResponse = new ChatCompletionResponse { Message = message, SessionId = state.SessionId, ChatHistory = chatHistory};

        return state;
    }
}