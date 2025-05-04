using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.ConsoleApp.Extensions;

public static class AgentExtensions
{
    public static void AddAgents(this IServiceCollection services, List<AgentSettings> agentSettings)
    {
        Verify.NotNull(agentSettings);
        
        services.AddKeyedSingleton<IAgent, TaskAgent>(AgentNames.TaskAgent);
    }
}