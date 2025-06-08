using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Todo.Agents;

public class Agent(ChatCompletionAgent chatCompletionAgent, string name) : AgentBase, IAgent
{
    public string Name { get; } = name;

    public override async Task<AgentState> InvokeAsync(AgentState state)
    {
        var chatHistory = state.Get<ChatHistoryAgentThread>("ChatHistory");

        await foreach (ChatMessageContent response in chatCompletionAgent.InvokeAsync(state.Request, chatHistory))
        {
            state.Responses.Add(response);
        }

        return state;
    }
}