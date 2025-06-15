using Microsoft.SemanticKernel;

namespace Todo.Agents;

public class AgentStreamingMessageHandler(Func<StreamingChatMessageContent, Task> messageCallback) : IAgentMessageHandler
{
    public async Task<string> Handle(StreamingChatMessageContent chatMessageContent)
    {
        await messageCallback(chatMessageContent);
        return chatMessageContent.Content!;
    }
}