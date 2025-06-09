using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Todo.Agents.Middleware;
using Todo.Agents.Settings;
using Todo.Infrastructure.Azure;

namespace Todo.Agents.Build;

public class AgentProvider(
    IAgentChatHistoryProvider agentChatHistoryProvider,
    IAgentTaskRepository agentTaskRepository,
    ILogger<IAgent> agentLogger,
    IAgentFactory agentFactory,
    IOptions<List<AgentSettings>> agentSettings) : IAgentProvider
{
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;

    public async Task<IAgent> Create(string name)
    {
        var agentBuild = new AgentMiddlewareBuilder();

        var agent = await agentFactory.Create(_agentSettings.First(x => x.Name == name));

        agentBuild.Use(new AgentExceptionHandlingMiddleware(agentLogger, agent.Name));
        agentBuild.Use(new AgentTaskMiddleware(agentLogger, agentTaskRepository));
        agentBuild.Use(new AgentTraceMiddleware(agent.Name));
        agentBuild.Use(new AgentConversationHistoryMiddleware(agentChatHistoryProvider, agent.Name));

        agentBuild.Use((IAgentMiddleware)agent);

        agentBuild.Use(new TerminationMiddleWare());

        return new AgentDelegateWrapper(agentBuild.Build(), agent.Name);
    }

}

public interface IAgentProvider
{
    Task<IAgent> Create(string name);
}
