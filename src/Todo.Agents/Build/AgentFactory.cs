using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Todo.Agents.Settings;
using Todo.Infrastructure.Azure;

namespace Todo.Agents.Build;

public class AgentFactory(
    Kernel kernel, 
    IPluginFactory pluginFactory,
    IAgentTemplateRepository agentTemplateRepository) : IAgentFactory
{
    public async Task<IAgent> Create(AgentSettings agentSetting)
    {
        var agentKernel = kernel.Clone();
        
        AddAgentFunctionsToKernel(agentSetting, agentKernel);
 
        var templateConfig = await CreateTemplateConfig(agentSetting.Template);

        var chatCompletionAgent = CreateChatCompletionAgent(templateConfig, agentSetting, agentKernel);

        return new Agent(chatCompletionAgent, agentSetting.Name);
    }

    private void AddAgentFunctionsToKernel(AgentSettings agentSetting, Kernel agentKernel)
    {
        foreach (var name in agentSetting.Plugins)
        {
            agentKernel.Plugins.AddFromObject(pluginFactory.Create(name, agentSetting.Name), name);
        }
    }

    private ChatCompletionAgent CreateChatCompletionAgent(PromptTemplateConfig template, AgentSettings agentSetting, Kernel agentKernel)
    {
        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ServiceId = agentSetting.ServiceId,
            ToolCallBehavior = GetToolCallBehavior(agentSetting.ToolCallBehavior),
            Temperature = double.Parse(agentSetting.Temperature)
        };

        return new ChatCompletionAgent(template, new KernelPromptTemplateFactory())
        {
            Kernel = agentKernel,
            Arguments = new KernelArguments(promptExecutionSettings)
        };
    }

    private static ToolCallBehavior GetToolCallBehavior(string toolCallBehavior)
    {
        return toolCallBehavior switch
        {
            "AutoInvokeKernelFunctions" => ToolCallBehavior.AutoInvokeKernelFunctions,
            _ => throw new InvalidOperationException($"Tool Call Behavior not valid {toolCallBehavior}")
        };
    }

    private async Task<PromptTemplateConfig> CreateTemplateConfig(string templateName)
    {
        var agentTemplate = await agentTemplateRepository.GetAgentTemplateAsync(templateName);

        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);

        return templateConfig;
    }
   
}

public interface IAgentFactory
{
   Task<IAgent> Create(AgentSettings configuration);
}