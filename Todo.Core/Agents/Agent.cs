using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Communication;

namespace Todo.Core.Agents;

#pragma warning disable SKEXP0110

public class Agent : IAgent
{
    private readonly IAgentChatHistoryProvider _agentChatHistoryProvider;
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public Agent(AgentConfiguration configuration, Kernel kernel, IAgentChatHistoryProvider agentChatHistoryProvider)
    {
        _agentChatHistoryProvider = agentChatHistoryProvider;
        var promptExecutionSettings = new PromptExecutionSettings
        {
            ServiceId = configuration.Settings.ServiceId,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()

        };

        _chatCompletionAgent = new ChatCompletionAgent(configuration.Template, configuration.PromptTemplateFactory)
        {
            Kernel = kernel.Clone(),
            Arguments = new KernelArguments(promptExecutionSettings)
        };
    }
    
    public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request)
    {
        var stringBuilder = new StringBuilder();
     
        var agentThread = await _agentChatHistoryProvider.LoadChatHistoryAsync($"{_chatCompletionAgent.Name}-{request.SessionId}");

        _chatCompletionAgent.Kernel.Data.Add("sessionId", request.SessionId);
      
        await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, request.Message), agentThread))
        {
            stringBuilder.AppendLine(response.Content);
        }

        await _agentChatHistoryProvider.SaveChatHistoryAsync(agentThread,
            $"{_chatCompletionAgent.Name}-{request.SessionId}");

        return new ChatCompletionResponse { Message = stringBuilder.ToString(), SessionId = request.SessionId };
    }
}