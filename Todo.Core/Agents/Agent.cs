using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Communication;

namespace Todo.Core.Agents;

public class Agent : IAgent
{
    private readonly ILogger<Agent> _logger;
    private const string AgentName = AgentNames.TaskAgent;
    
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

    public async Task<AgentTask> InvokeAsync(AgentTask agentTask)
    {
        try
        {
            var userInput = agentTask.History.First().Message;

            await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, userInput)))
            {
              agentTask.Artifacts.Add(new AgentArtifact() {Message = response.Content!});  
            }

            return agentTask;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while processing the chat input.");
            throw;
        }
    }
}