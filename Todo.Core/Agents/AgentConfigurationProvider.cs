using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Todo.Core.Infrastructure;
using Todo.Core.Settings;

namespace Todo.Core.Agents;

public class AgentConfigurationProvider(IAgentTemplateRepository agentTemplateRepository, IOptions<List<AgentSettings>> agentSettings) : IAgentConfigurationProvider
{
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;
    private readonly ConcurrentDictionary<AgentNames, AgentConfiguration> _agentTemplates = new();

    public AgentConfiguration GetConfiguration(AgentNames agentName)
    {
        if (_agentTemplates.TryGetValue(agentName, out var agentConfiguration))
        {
            return agentConfiguration;
        }
        throw new KeyNotFoundException($"Agent configuration for {agentName} not found.");
    }

    public async Task Load()
    {
        foreach (var agentSetting in _agentSettings)
        {
            var templateConfig = await LoadAgentTemplate(agentSetting.Template);

            var configuration = new AgentConfiguration { Settings = agentSetting, Template = templateConfig };

            if (!_agentTemplates.TryAdd(agentSetting.Name, configuration))
            {
                throw new InvalidOperationException($"Failed to add template for agent: {agentSetting.Name}. It may already exist.");
            }
        }
    }

    private async Task<PromptTemplateConfig> LoadAgentTemplate(string templateName)
    {
        var agentTemplate = await agentTemplateRepository.GetAgentTemplateAsync(templateName);
 
       return KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);
    }
}

public interface IAgentConfigurationProvider
{
    Task Load();
    AgentConfiguration GetConfiguration(AgentNames agentName);
}