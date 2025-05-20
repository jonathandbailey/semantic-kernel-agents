using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Todo.Core.Agents.Plugins;
using Todo.Core.Infrastructure;
using Todo.Core.Settings;

namespace Todo.Core.Agents.Build;

public class AgentFactory(
    Kernel kernel, 
    IAgentTemplateRepository agentTemplateRepository) : IAgentFactory
{
    public async Task<IAgent> Create(AgentSettings agentSetting, IAgentProvider agentProvider)
    {
        var agentKernel = kernel.Clone();
        
        foreach (var name in agentSetting.Plugins)
        {
            switch (name)
            {
                case "TaskPlugin":
                    agentKernel.Plugins.AddFromObject(new TaskPlugin(agentProvider), "TaskPlugin");
                    break;
            }
        }

        var agentTemplate = await agentTemplateRepository.GetAgentTemplateAsync(agentSetting.Template);

        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);

        var chatCompletionAgent = Create(templateConfig, agentSetting, agentKernel);

        return new Agent(chatCompletionAgent, agentSetting.Name);
    }
    
    private ChatCompletionAgent Create(PromptTemplateConfig template, AgentSettings agentSetting, Kernel agentKernel)
    {
        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ServiceId = agentSetting.ServiceId,
            ToolCallBehavior = Get(agentSetting.ToolCallBehavior),
            Temperature = double.Parse(agentSetting.Temperature)

        };

        return new ChatCompletionAgent(template, new KernelPromptTemplateFactory())
        {
            Kernel = agentKernel,
            Arguments = new KernelArguments(promptExecutionSettings)
        };
    }

    private ToolCallBehavior Get(string toolCallBehavior)
    {
        return toolCallBehavior switch
        {
            "AutoInvokeKernelFunctions" => ToolCallBehavior.AutoInvokeKernelFunctions,
            _ => throw new InvalidOperationException($"Tool Call Behavior not valid {toolCallBehavior}")
        };
    }
   
}

public interface IAgentFactory
{
   Task<IAgent> Create(AgentSettings configuration, IAgentProvider agentProvider);
}