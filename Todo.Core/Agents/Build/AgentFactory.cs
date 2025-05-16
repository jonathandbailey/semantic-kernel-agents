using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Todo.Core.Agents.Plugins;
using Todo.Core.Infrastructure;
using Todo.Core.Settings;

namespace Todo.Core.Agents.Build;

public class AgentFactory(
    IAgentChatHistoryProvider agentChatHistoryProvider, 
    Kernel kernel, 
    IAgentTemplateRepository agentTemplateRepository,
    ILogger<Agent> logger) : IAgentFactory
{
    public async Task<IAgent> Create(AgentSettings agentSetting, IAgentProvider agentProvider)
    {
        var configuration = await Load(agentSetting);
        
        var clonedKernel = kernel.Clone();
            
        foreach (var name in configuration.Settings.Plugins)
        {
            switch (name)
            {
                case "TaskPlugin":
                    clonedKernel.Plugins.AddFromObject(new TaskPlugin(agentProvider), "TaskPlugin");
                    break;
            }
        }

        return new Agent(configuration, clonedKernel, agentChatHistoryProvider, logger);
    }

    private async Task<AgentConfiguration> Load(AgentSettings agentSetting)
    {
        var agentTemplate = await agentTemplateRepository.GetAgentTemplateAsync(agentSetting.Template);

        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);

        return new AgentConfiguration { Settings = agentSetting, Template = templateConfig };
    }
   
}

public interface IAgentFactory
{
   Task<IAgent> Create(AgentSettings configuration, IAgentProvider agentProvider);
}