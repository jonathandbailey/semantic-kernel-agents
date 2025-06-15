using Microsoft.SemanticKernel;

namespace Todo.Agents;


public class AgentMessageHandler : IAgentMessageHandler
{
    public Task<string> Handle(StreamingChatMessageContent chatMessageContent)
    {
        return Task.FromResult(chatMessageContent.Content!);
    }
}

public interface IAgentMessageHandler
{
    Task<string> Handle(StreamingChatMessageContent chatMessageContent);
}
