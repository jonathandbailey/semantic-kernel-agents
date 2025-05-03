using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Settings;

namespace Todo.Core.Agents;

public class TaskAgent : IAgent
{
    private const AgentNames AgentName = AgentNames.TaskAgent;
    
    private readonly ChatCompletionAgent _chatCompletionAgent;

    public TaskAgent(
        IAgentTemplateProvider agentTemplateProvider, 
        Kernel kernel, 
        [FromKeyedServices(AgentNames.TaskAgent)] AgentSettings settings)
    {
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
        await foreach (ChatMessageContent response in _chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, userInput)))
        {
            Console.WriteLine(response.Content);
        }
    }
}