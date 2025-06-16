using Microsoft.SemanticKernel;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Todo.Agents;

public class AgentStreamingMessageHandler(Func<StreamingChatMessageContent, bool, Task> messageCallback) : IAgentMessageHandler
{
    private const string StatusHeaderRegex = @"^\[status:\s*.*?\]\s*";
    private readonly StringBuilder _buffer = new();
    private bool _isHeaderRemoved;

    public async Task<string> Handle(StreamingChatMessageContent chatMessageContent)
    {
        if (_isHeaderRemoved)
        {
            await messageCallback(chatMessageContent, false);
            return chatMessageContent.Content!;
        }

        _buffer.Append(chatMessageContent.Content);
        
        var combined = _buffer.ToString();
        var match = Regex.Match(combined, StatusHeaderRegex);

        if (!match.Success) return chatMessageContent.Content!;
        
        _isHeaderRemoved = true;
        
        var headerEnd = match.Index + match.Length;
        var content = combined.Substring(headerEnd);
        
        _buffer.Clear();


        if (string.IsNullOrEmpty(content)) return content;
        
        var contentMessage = new StreamingChatMessageContent(chatMessageContent.Role, content);
        await messageCallback(contentMessage, false);

        return content;
    }

    public async Task<string> FlushMessages()
    {
        var contentMessage = new StreamingChatMessageContent(AuthorRole.Assistant, string.Empty);
        await messageCallback(contentMessage, true);
        return string.Empty;
    }
}