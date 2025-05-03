using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Todo.Core.Infrastructure;
using Todo.Core.Settings;

namespace Todo.Core.Agents;

public class AgentTemplateProvider(IAgentTemplateRepository agentTemplateRepository, IOptions<List<AgentSettings>> agentSettings) : IAgentTemplateProvider
{
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;
    private readonly Dictionary<AgentNames, PromptTemplateConfig> _agentTemplates = new();
        
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

            _agentTemplates.Add(agentSetting.Name, templateConfig);
        }
    }

    private async Task<PromptTemplateConfig> LoadAgentTemplate(string templateName)
    {
        var agentTemplate = await agentTemplateRepository.GetAgentTemplateAsync(templateName);

        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);

        return templateConfig;
    }
}

public interface IAgentTemplateProvider
{
    PromptTemplateConfig Get(AgentNames agentName);
    Task LoadAgentTemplates();
}