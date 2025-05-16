using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Todo.Core.Communication;

namespace Todo.Core.Agents;

#pragma warning disable SKEXP0110

public class Agent : IAgent
{
    private readonly IAgentChatHistoryProvider _agentChatHistoryProvider;
    private readonly ILogger<Agent> _logger;
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public Agent(AgentConfiguration configuration, Kernel kernel, IAgentChatHistoryProvider agentChatHistoryProvider, ILogger<Agent> logger)
    {
        _agentChatHistoryProvider = agentChatHistoryProvider;
        _logger = logger;
        var promptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            ServiceId = configuration.Settings.ServiceId,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 0.0f
        };

        _chatCompletionAgent = new ChatCompletionAgent(configuration.Template, configuration.PromptTemplateFactory)
        {
            Kernel = kernel.Clone(),
            Arguments = new KernelArguments(promptExecutionSettings)
        };
    }
    
    public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest request)
    {
        try
        {
            var stringBuilder = new StringBuilder();
     
            
            //TODO : Chat History should be loaded in Middleware
            var agentThread = await _agentChatHistoryProvider.LoadChatHistoryAsync($"{request.SessionId} - [{_chatCompletionAgent.Name}]");

            _chatCompletionAgent.Kernel.Data.Add("sessionId", request.SessionId);
      
            await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, request.Message), agentThread))
            {
                stringBuilder.AppendLine(response.Content);
            }

            var message = stringBuilder.ToString();

            await _agentChatHistoryProvider.SaveChatHistoryAsync(agentThread,
                $"{request.SessionId} - [{_chatCompletionAgent.Name}]");

            return new ChatCompletionResponse { Message = message, SessionId = request.SessionId };
        }
        catch (Exception e)
        {
            _logger.LogError($"Unknown Error in Agent : {_chatCompletionAgent.Name} :{e.Message}", e);
            throw;
            // TODO : Exception should not be thrown, could be handled in middleware
            // TODO : Agent should update return value with an Error message
        }
    }
}