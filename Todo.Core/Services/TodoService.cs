using Todo.Core.Agents;

namespace Todo.Core.Services;

public class TodoService(IAgentProvider agentProvider) : ITodoService
{
    public async Task Chat(string userInput)
    {
        var agent = await agentProvider.Get(AgentNames.TaskAgent);

        await agent.Chat(userInput);
    }
}

public interface ITodoService
{
    Task Chat(string userInput);
}