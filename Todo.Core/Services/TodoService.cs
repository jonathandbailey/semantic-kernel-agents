using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;

namespace Todo.Core.Services;

public class TodoService([FromKeyedServices(AgentNames.TaskAgent)] IAgent agent) : ITodoService
{
    public async Task Chat(string userInput)
    {
        await agent.Chat(userInput);
    }
}

public interface ITodoService
{
    Task Chat(string userInput);
}