using Microsoft.Extensions.DependencyInjection;

namespace Todo.Core.Agents;
public class AgentProvider(IServiceProvider serviceProvider, IAgentConfigurationProvider agentConfigurationProvider) : IAgentProvider
{
    public async Task Build()
    {
        await agentConfigurationProvider.Load();
    }
    
    public IAgent Get()
    {
        return serviceProvider.GetRequiredKeyedService<IAgent>(AgentNames.TaskAgent);
    }
}

public interface IAgentProvider
{
    IAgent Get();
    Task Build();
}
