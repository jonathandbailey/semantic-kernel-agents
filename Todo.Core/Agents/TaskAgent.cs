using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Todo.Core.Agents;

public class TaskAgent(ChatCompletionAgent chatCompletionAgent) : IAgent
{
    public async Task Chat(string userInput)
    {
        await foreach (ChatMessageContent response in chatCompletionAgent.InvokeAsync(new ChatMessageContent(AuthorRole.User, userInput)))
        {
            Console.WriteLine(response.Content);
        }
    }
}