using Microsoft.SemanticKernel;

namespace Todo.Agents;


public class AgentMessageHandler : IAgentMessageHandler
{
    public Task<string> Handle(StreamingChatMessageContent chatMessageContent)
    {
        return Task.FromResult(chatMessageContent.Content!);
    }

    public Task<string> FlushMessages()
    {
        return Task.FromResult(string.Empty);
    }
}

public interface IAgentMessageHandler
{
    Task<string> Handle(StreamingChatMessageContent chatMessageContent);
    Task<string> FlushMessages();
}
