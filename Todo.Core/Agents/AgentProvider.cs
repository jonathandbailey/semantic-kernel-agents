using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.SemanticKernel;
using Todo.Core.Settings;
using Todo.Core.Infrastructure;
using Todo.Core.Middleware;
using Todo.Core.Plugins;

namespace Todo.Core.Agents;
public class AgentProvider(
    Kernel kernel, 
    IAgentTemplateRepository agentTemplateRepository, 
    IChatHistoryRepository chatHistoryRepository,
    IOptions<List<AgentSettings>> agentSettings) : IAgentProvider
{
    private readonly ConcurrentDictionary<string, IAgentTaskManager> _agents = new();
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;

    public async Task Build()
    {
        foreach (var agentSetting in _agentSettings)
        {
            var configuration = await Load(agentSetting);
        
            var agent = BuildAgentMiddleware(configuration);

            if (!_agents.TryAdd(agentSetting.Name, new AgentTaskManager(agent)))
            {
                throw new InvalidOperationException($"Failed to add agent: {agentSetting.Name}. It may already exist.");
            }
        }
    }
    
    public IAgentTaskManager GetTaskManager(string name)
    {
        if (_agents.TryGetValue(name, out var agent))
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

        var agentKernel = kernel.Clone();

        foreach (var name in configuration.Settings.Plugins)
        {
            switch (name)
            {
                case "TaskPlugin":
                    agentKernel.Plugins.AddFromObject(new TaskPlugin(this), "TaskPlugin");
                    break;
            }
        }

        var agent = new Agent(configuration, agentKernel, chatHistoryRepository);

        agentBuild.Use(new AgentMiddleware(agent));

        return new AgentDelegateWrapper(agentBuild.Build());
    }
}

public interface IAgentProvider
{
    IAgentTaskManager GetTaskManager(string name);
    Task Build();
}
