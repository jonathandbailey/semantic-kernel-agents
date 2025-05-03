using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Todo.Core.Infrastructure;
using Todo.Core.Settings;

namespace Todo.Core.Agents;

public class AgentTemplateProvider(IAgentTemplateRepository agentTemplateRepository, IOptions<List<AgentSettings>> agentSettings) : IAgentTemplateProvider
{
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;
    private readonly ConcurrentDictionary<AgentNames, PromptTemplateConfig> _agentTemplates = new();
        
    public PromptTemplateConfig Get(AgentNames agentName)
    {
        if (_agentTemplates.TryGetValue(agentName, out var templateConfig))
        {
            return templateConfig;
        }
        throw new KeyNotFoundException($"Agent template for {agentName} not found.");
    }

    public async Task LoadAgentTemplates()
    {
        foreach (var agentSetting in _agentSettings)
        {
            var templateConfig = await LoadAgentTemplate(agentSetting.Template);

            if (!_agentTemplates.TryAdd(agentSetting.Name, templateConfig))
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

public interface IAgentTemplateProvider
{
    PromptTemplateConfig Get(AgentNames agentName);
    Task LoadAgentTemplates();
}