using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Todo.Core.Infrastructure;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Agents;

public class AgentProvider(IOptions<List<AgentSettings>> agentSettings, Kernel kernel, IAgentTemplateRepository agentTemplateRepository) : IAgentProvider
{
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;

    private readonly Dictionary<string, Func<ChatCompletionAgent, IAgent>> _factory =
        new()
        {
            {AgentNames.TaskAgent, agent => new TaskAgent(agent) }
        };


    public async Task<IAgent> Get(string agentName)
    {
        Verify.NotNullOrWhiteSpace(agentName);

        var agentSetting = GetAgentSettings(agentName);

        var agent = await Create(agentSetting);

        return agent;
    }

    private async Task<IAgent> Create(AgentSettings settings)
    {
        var templateConfig = await GetAgentTemplate(settings.Template);

        var agent = new ChatCompletionAgent(templateConfig, new KernelPromptTemplateFactory())
        {
            Kernel = kernel.Clone(),
            Arguments =
                new KernelArguments(
                    new PromptExecutionSettings()
                    {
                        ServiceId = settings.ServiceId,
                    })
        };

        if (!_factory.TryGetValue(settings.Name, out var value))
        {
            throw new InvalidOperationException($"Agent type '{settings.Name}' is not supported.");
        }

        return value(agent);
    }

    private AgentSettings GetAgentSettings(string agentName)
    {
        var agentSetting = _agentSettings.FirstOrDefault(x => x.Name == agentName);

        return agentSetting ?? throw new ArgumentException($"Agent with name {agentName} not found");
    }

    private async Task<PromptTemplateConfig> GetAgentTemplate(string templateName)
    {
        var agentTemplate = await agentTemplateRepository.GetAgentTemplateAsync(templateName);

        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);

        return templateConfig;
    }
}

public interface IAgentProvider
{
    Task<IAgent> Get(string agentName);
}