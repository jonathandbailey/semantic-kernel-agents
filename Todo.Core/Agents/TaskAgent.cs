using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Settings;
using Todo.Core.Users;

namespace Todo.Core.Agents;

public class TaskAgent : IAgent
{
    private readonly IUser _user;
    private readonly ILogger<TaskAgent> _logger;
    private const AgentNames AgentName = AgentNames.TaskAgent;
    
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public TaskAgent(
        IUser user,
        IAgentTemplateProvider agentTemplateProvider, 
        Kernel kernel, 
        [FromKeyedServices(AgentNames.TaskAgent)] AgentSettings settings,
        ILogger<TaskAgent> logger)
    {
        _user = user;
        _logger = logger;
        
        var templateConfig = agentTemplateProvider.Get(AgentName);

        var promptExecutionSettings = new PromptExecutionSettings
        {
            ServiceId = settings.ServiceId
        };

        _chatCompletionAgent = new ChatCompletionAgent(templateConfig, new KernelPromptTemplateFactory())
        {
            Kernel = kernel.Clone(),
            Arguments = new KernelArguments(promptExecutionSettings)
        };
    }

    public async Task Chat(string userInput)
    {
        try
        {
            await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, userInput)))
            {
                await _user.Reply(response.Content!);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while processing the chat input.");
            throw;
        }
    }
}