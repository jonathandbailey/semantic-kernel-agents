using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agents;

public class AgentStreamingMessageHandler(Func<StreamingChatMessageContent, bool, Guid, Task> messageCallback) : IAgentMessageHandler
{
    private readonly StringBuilder _buffer = new();
    private bool _isHeaderRemoved;
    private bool _canStream = true;
    private Guid _requestId;
   
    public async Task<string> Handle(StreamingChatMessageContent chatMessageContent, Guid requestId)
    {
        _requestId = requestId;
        
        _buffer.Append(chatMessageContent.Content);

        if (!_canStream) return chatMessageContent.Content!;


        if (_isHeaderRemoved)
        {
            await messageCallback(chatMessageContent, false, requestId);
            return chatMessageContent.Content!;
        }

        if(!AgentHeaderParser.HasStartEndHeaders(_buffer.ToString())) return chatMessageContent.Content!;

        _isHeaderRemoved = true;

        if (!AgentHeaderParser.HasHeader(AgentHeaderParser.StreamToUserHeader, _buffer.ToString() ))
        {
            _canStream = false;
            return chatMessageContent.Content!;
        }

        var content = AgentHeaderParser.RemoveHeaders(_buffer.ToString());
       
  
        var contentMessage = new StreamingChatMessageContent(chatMessageContent.Role, content);
        await messageCallback(contentMessage, false, requestId);

        return content;
    }

    public async Task<string> FlushMessages()
    {
        var contentMessage = new StreamingChatMessageContent(AuthorRole.Assistant, string.Empty);
        await messageCallback(contentMessage, true, _requestId);

        var content = _buffer.ToString();

        _buffer.Clear();

        return content;
    }
}