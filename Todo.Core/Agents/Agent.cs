using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Todo.Core.Agents;

public class Agent : IAgent
{
    private readonly ILogger<Agent> _logger;
    private const AgentNames AgentName = AgentNames.TaskAgent;
    
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public Agent(IAgentConfigurationProvider agentConfigurationProvider, 
        Kernel kernel,
        ILogger<Agent> logger)
    {
        _logger = logger;
       
        var configuration = agentConfigurationProvider.GetConfiguration(AgentName);

        var promptExecutionSettings = new PromptExecutionSettings
        {
            ServiceId = configuration.Settings.ServiceId,
        };

        _chatCompletionAgent = new ChatCompletionAgent(configuration.Template, configuration.PromptTemplateFactory)
        {
            Kernel = kernel.Clone(),
            Arguments = new KernelArguments(promptExecutionSettings)
        };
    }

    public async Task<IEnumerable<string>> InvokeAsync(string userInput)
    {
        try
        {
            var responses = new List<string>();
            
            await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, userInput)))
            {
                responses.Add(response.Content!);
            }

            return responses;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while processing the chat input.");
            throw;
        }
    }
}