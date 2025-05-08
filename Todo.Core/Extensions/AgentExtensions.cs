using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;

namespace Todo.Core.Extensions;

public static class AgentExtensions
{
    public static void AddAgents(this IServiceCollection services)
    {
        services.AddKeyedScoped<IAgent, TaskAgent>(AgentNames.TaskAgent);
    }
}