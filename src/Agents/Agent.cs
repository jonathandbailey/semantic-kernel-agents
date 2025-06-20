using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agents;

public class Agent(ChatCompletionAgent chatCompletionAgent, string name, IAgentMessageHandler agentMessageHandler) : AgentBase, IAgent
{
    public string Name { get; } = name;

    public override async Task<AgentState> InvokeAsync(AgentState state)
    {
        var chatHistory = state.Get<ChatHistoryAgentThread>("ChatHistory");

        var stringBuilder = new StringBuilder();

        var kernelArguments = new KernelArguments();
        
        foreach (var argument in state.Arguments)
        {
            kernelArguments.Add(argument.Key, argument.Value);
        }

        var agentInvokeOptions = new AgentInvokeOptions { KernelArguments = kernelArguments };

        await foreach (var response in chatCompletionAgent.InvokeStreamingAsync(state.Request, chatHistory, options: agentInvokeOptions))
        {
            stringBuilder.Append(await agentMessageHandler.Handle(response.Message));
        }

        var content = await agentMessageHandler.FlushMessages();

        state.Responses.Add(new ChatMessageContent(AuthorRole.Assistant, content));

        return state;
    }
}