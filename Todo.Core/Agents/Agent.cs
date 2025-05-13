using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Communication;
using Todo.Core.Infrastructure;

namespace Todo.Core.Agents;

#pragma warning disable SKEXP0110

public class Agent : IAgent
{
    private readonly IChatHistoryRepository _chatHistoryRepository;
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public Agent(AgentConfiguration configuration, Kernel kernel, IChatHistoryRepository chatHistoryRepository)
    {
        
        _chatHistoryRepository = chatHistoryRepository;

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

        ChatHistoryAgentThread agentThread = new();
        
        await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, request.Message), agentThread ))
        {
            stringBuilder.AppendLine(response.Content);
        }

        var messages = new List<ChatMessageContent>();
        
        await foreach (var msg in agentThread.GetMessagesAsync())
        {
            messages.Add(msg);
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(messages, options);

        await _chatHistoryRepository.SaveChatHistoryAsync(request.SessionId, json);

        return new ChatCompletionResponse { Message = stringBuilder.ToString(), SessionId = request.SessionId };
    }
}