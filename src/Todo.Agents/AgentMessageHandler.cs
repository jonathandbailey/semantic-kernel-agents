using Microsoft.SemanticKernel;
using System.Text;

namespace Todo.Agents;


public class AgentMessageHandler : IAgentMessageHandler
{
    private readonly StringBuilder _buffer = new();

    public Task<string> Handle(StreamingChatMessageContent chatMessageContent)
    {
        _buffer.Append(chatMessageContent.Content);

        return Task.FromResult(chatMessageContent.Content!);
    }

    public Task<string> FlushMessages()
    {
        var content = _buffer.ToString();

        _buffer.Clear();

        return Task.FromResult(content);
    }
}

public interface IAgentMessageHandler
{
    Task<string> Handle(StreamingChatMessageContent chatMessageContent);
    Task<string> FlushMessages();
}
