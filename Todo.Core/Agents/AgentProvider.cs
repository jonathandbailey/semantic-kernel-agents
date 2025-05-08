using Microsoft.Extensions.DependencyInjection;

namespace Todo.Core.Agents;
public class AgentProvider(IServiceProvider serviceProvider) : IAgentProvider
{
    public IAgent Get()
    {
        return serviceProvider.GetRequiredKeyedService<IAgent>(AgentNames.TaskAgent);
    }
}

public interface IAgentProvider
{
    IAgent Get();
}
