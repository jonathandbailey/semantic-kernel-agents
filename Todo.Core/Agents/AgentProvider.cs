using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.SemanticKernel;
using Todo.Core.Settings;
using Todo.Core.Infrastructure;
using Todo.Core.Middleware;

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
        
            var agent = BuildAgentMiddleware(configuration);

            if (!_agents.TryAdd(agentSetting.Name, agent))
            {
                throw new InvalidOperationException($"Failed to add agent: {agentSetting.Name}. It may already exist.");
            }
        }
    }
    
    public IAgent Get()
    {
        if (_agents.TryGetValue(AgentNames.OrchestratorAgent, out var agent))
        {
            return agent;
        }
        
        throw new KeyNotFoundException($"Agent not found: {AgentNames.OrchestratorAgent}");
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

    private AgentDelegateWrapper BuildAgentMiddleware(AgentConfiguration configuration)
    {
        var agentBuild = new AgentMiddlewareBuilder();

        var agent = new Agent(configuration, kernel);

        agentBuild.Use(new AgentMiddleware(agent));

        return new AgentDelegateWrapper(agentBuild.Build());
    }
}

public interface IAgentProvider
{
    IAgent Get();
    Task Build();
}
