using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Todo.Agents;

public class Agent(ChatCompletionAgent chatCompletionAgent, string name, IAgentMessageHandler agentMessageHandler) : AgentBase, IAgent
{
    public string Name { get; } = name;

    public override async Task<AgentState> InvokeAsync(AgentState state)
    {
        var chatHistory = state.Get<ChatHistoryAgentThread>("ChatHistory");

        var stringBuilder = new StringBuilder();

        await foreach (var response in chatCompletionAgent.InvokeStreamingAsync(state.Request, chatHistory))
        {
            stringBuilder.Append(await agentMessageHandler.Handle(response.Message));
        }

        state.Responses.Add(new ChatMessageContent(AuthorRole.Assistant, stringBuilder.ToString()));

        await agentMessageHandler.FlushMessages();

        return state;
    }
}