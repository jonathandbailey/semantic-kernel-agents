using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Todo.Core.Settings;
using Todo.Core.Agents.Middleware;

namespace Todo.Core.Agents.Build;
public class AgentProvider(
    ILogger<AgentTaskManager> taskManagerLogger,
    IAgentChatHistoryProvider agentChatHistoryProvider,
    ILogger<IAgent> agentLogger,
    IAgentFactory agentFactory,
    IOptions<List<AgentSettings>> agentSettings) : IAgentProvider
{
    private readonly ConcurrentDictionary<string, IAgentTaskManager> _agents = new();
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;

    public async Task Build()
    {
        foreach (var agentSetting in _agentSettings)
        {
            var agent = await BuildAgentMiddleware(agentSetting);

            if (!_agents.TryAdd(agentSetting.Name, new AgentTaskManager(agent, taskManagerLogger)))
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
   
    private async Task<AgentDelegateWrapper> BuildAgentMiddleware(AgentSettings configuration)
    {
        var agentBuild = new AgentMiddlewareBuilder();

        var agent = await agentFactory.Create(configuration, this);

        agentBuild.Use(new AgentExceptionHandlingMiddleware(agentLogger, agent.Name));
        agentBuild.Use(new AgentTraceMiddleware(agent.Name));
        agentBuild.Use(new AgentConversationHistoryMiddleware(agentChatHistoryProvider, agent.Name));
        
        agentBuild.Use((IAgentMiddleware) agent);

        return new AgentDelegateWrapper(agentBuild.Build(), agent.Name);
    }
}

public interface IAgentProvider
{
    IAgentTaskManager GetTaskManager(string name);
    Task Build();
}
