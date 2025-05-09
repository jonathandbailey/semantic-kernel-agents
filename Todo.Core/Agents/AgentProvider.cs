using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.SemanticKernel;
using Todo.Core.Settings;
using Todo.Core.Infrastructure;

namespace Todo.Core.Agents;
public class AgentProvider(Kernel kernel, IAgentTemplateRepository agentTemplateRepository, IOptions<List<AgentSettings>> agentSettings) : IAgentProvider
{
    private readonly ConcurrentDictionary<string, IAgent> _agents = new();
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;

    public async Task Build()
    {
        foreach (var agentSetting in _agentSettings)
        {
            var configuration = await Load(agentSetting);
        
            if (!_agents.TryAdd(agentSetting.Name, new Agent(configuration, kernel)))
            {
                throw new InvalidOperationException($"Failed to add agent: {agentSetting.Name}. It may already exist.");
            }
        }
    }
    
    public IAgent Get()
    {
        if (_agents.TryGetValue(AgentNames.TaskAgent, out var agentConfiguration))
        {
            return agentConfiguration;
        }
        throw new KeyNotFoundException($"Agent not found.");
    }

    private async Task<AgentConfiguration> Load(AgentSettings agentSetting)
    {
        var templateConfig = await LoadAgentTemplate(agentSetting.Template);

        return new AgentConfiguration { Settings = agentSetting, Template = templateConfig };
    }

    private async Task<PromptTemplateConfig> LoadAgentTemplate(string templateName)
    {
        var agentTemplate = await agentTemplateRepository.GetAgentTemplateAsync(templateName);

        return KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);
    }
}

public interface IAgentProvider
{
    IAgent Get();
    Task Build();
}
