using Agents.Middleware;
using Agents.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Todo.Core.Users;
using Todo.Core.Vacations;

namespace Agents.Build;

public class AgentProvider(
    IAgentChatHistoryProvider agentChatHistoryProvider,
    ILogger<IAgent> agentLogger,
    IAgentFactory agentFactory,
    IUserMessageSender userMessageSender,
    IVacationPlanService vacationPlanService,
    IOptions<List<AgentSettings>> agentSettings) : IAgentProvider
{
    private readonly List<AgentSettings> _agentSettings = agentSettings.Value;

    public async Task<IAgent> Create(string name)
    {
        var agent = await agentFactory.Create(_agentSettings.First(x => x.Name == name), StreamingMessageCallback);

        return BuildAgentMiddleware(agent);
    }

    private async Task StreamingMessageCallback(StreamingChatMessageContent streamingChatMessageContent, bool isEndOfStream, Guid id)
    {
        if (streamingChatMessageContent.Content != null)
        {
            await userMessageSender.StreamingMessageCallback(streamingChatMessageContent.Content!, isEndOfStream, id);
        }
    }

    private IAgent BuildAgentMiddleware(IAgent agent)
    {
        var agentBuild = new AgentMiddlewareBuilder();
      
        agentBuild.Use(new AgentExceptionHandlingMiddleware(agentLogger, agent.Name));
        agentBuild.Use(new AgentTraceMiddleware(agent.Name));

        if (agent.Name == AgentNames.OrchestratorAgent)
        {
            agentBuild.Use(new TravelDataMiddleware(vacationPlanService));
        }

        if (agent.Name == AgentNames.AccommodationAgent)
        {
            agentBuild.Use(new TaskParserMiddleware());
        }

        agentBuild.Use(new AgentConversationsMiddleware(agentChatHistoryProvider));
        agentBuild.Use(new AgentChatHistoryMiddleware(agentChatHistoryProvider, agent.Name));

        agentBuild.Use((IAgentMiddleware)agent);

        agentBuild.Use(new TerminationMiddleWare());

        return new AgentDelegateWrapper(agentBuild.Build(), agent.Name);
    }

}

public interface IAgentProvider
{
    Task<IAgent> Create(string name);
}
