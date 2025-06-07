using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Todo.Agents.Communication;
using Todo.Agents.Middleware;
using Todo.Agents.Settings;
using Todo.Infrastructure.Azure;

namespace Todo.Agents.Build;
public class AgentProvider(
    ILogger<AgentTaskManager> taskManagerLogger,
    IAgentChatHistoryProvider agentChatHistoryProvider,
    IAgentTaskRepository agentTaskRepository,
    IAgentPublisher publisher,
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

            if (!_agents.TryAdd(agentSetting.Name, new AgentTaskManager(agent, taskManagerLogger, agentTaskRepository)))
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
        if (configuration.Type == "Worker")
        {
            return await BuildAgentMiddlewareWorker(configuration);
        }

        if (configuration.Type == "Orchestration")
        {
            return await BuildAgentMiddlewareOrchestrator(configuration);
        }

        throw new ArgumentException($"Agent Type : {configuration.Type} is not recognized");
    }

    private async Task<AgentDelegateWrapper> BuildAgentMiddlewareWorker(AgentSettings configuration)
    {
        var agentBuild = new AgentMiddlewareBuilder();

        var agent = await agentFactory.Create(configuration);

        agentBuild.Use(new AgentExceptionHandlingMiddleware(agentLogger, agent.Name));
        agentBuild.Use(new AgentTraceMiddleware(agent.Name));
        agentBuild.Use(new AgentConversationHistoryMiddleware(agentChatHistoryProvider, agent.Name));

        agentBuild.Use((IAgentMiddleware)agent);
        
        agentBuild.Use(new AgentResponseMiddleware(agentLogger, agent.Name));
        agentBuild.Use(new TerminationMiddleWare());

        return new AgentDelegateWrapper(agentBuild.Build(), agent.Name);
    }

    private async Task<AgentDelegateWrapper> BuildAgentMiddlewareOrchestrator(AgentSettings configuration)
    {
        var agentBuild = new AgentMiddlewareBuilder();

        var agent = await agentFactory.Create(configuration);

        agentBuild.Use(new AgentExceptionHandlingMiddleware(agentLogger, agent.Name));
        agentBuild.Use(new AgentTraceMiddleware(agent.Name));
        agentBuild.Use(new AgentConversationHistoryMiddleware(agentChatHistoryProvider, agent.Name));

        agentBuild.Use((IAgentMiddleware)agent);
       
        agentBuild.Use(new Agent2AgentMiddleWare(agentLogger, agent.Name, publisher));
        agentBuild.Use(new TerminationMiddleWare());

        return new AgentDelegateWrapper(agentBuild.Build(), agent.Name);
    }
}

public interface IAgentProvider
{
    IAgentTaskManager GetTaskManager(string name);
    Task Build();
}
