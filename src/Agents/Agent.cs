using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agents;

public class Agent(ChatCompletionAgent chatCompletionAgent, string name, IAgentMessageHandler agentMessageHandler) : AgentBase, IAgent
{
    public string Name { get; } = name;

    public override async Task<AgentState> InvokeAsync(AgentState state)
    {
        var chatHistory = state.AgentThread;
   
        var kernelArguments = new KernelArguments();
        
        foreach (var argument in state.Arguments)
        {
            kernelArguments.Add(argument.Key, argument.Value);
        }

        var agentInvokeOptions = new AgentInvokeOptions { KernelArguments = kernelArguments };

        await foreach (var response in chatCompletionAgent.InvokeStreamingAsync(state.Request, chatHistory, options: agentInvokeOptions))
        {
            await agentMessageHandler.Handle(response.Message, state.RequestId);
        }

        var content = await agentMessageHandler.FlushMessages();

        state.Response = new ChatMessageContent(AuthorRole.Assistant, content);

        return state;
    }
}