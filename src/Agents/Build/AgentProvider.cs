using Agents.Middleware;
using Agents.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace Agents.Build;

public class AgentProvider(
    IAgentChatHistoryProvider agentChatHistoryProvider,
    ILogger<IAgent> agentLogger,
    IAgentFactory agentFactory,
    IOptions<List<AgentSettings>> agentSettings) : IAgentProvider
{
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;

    public async Task<IAgent> Create(string name)
    {
        var agent = await agentFactory.Create(_agentSettings.First(x => x.Name == name));

        return BuildAgentMiddleware(agent);
    }

    public async Task<IAgent> Create(string name, Func<StreamingChatMessageContent, bool, Task> streamingMessageCallback)
    {
        var agent = await agentFactory.Create(_agentSettings.First(x => x.Name == name), streamingMessageCallback);

        return BuildAgentMiddleware(agent);
    }

    private IAgent BuildAgentMiddleware(IAgent agent)
    {
        var agentBuild = new AgentMiddlewareBuilder();
      
        agentBuild.Use(new AgentExceptionHandlingMiddleware(agentLogger, agent.Name));
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
    Task<IAgent> Create(string name, Func<StreamingChatMessageContent, bool, Task> streamingMessageCallback);
}
