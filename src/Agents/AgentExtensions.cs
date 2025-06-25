using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.A2A;
using Todo.Infrastructure;

namespace Agents;

public static class AgentExtensions
{
    public static ChatMessageContent ToChatMessageContent(this Message message)
    {
        return new ChatMessageContent
        {
            Role = new AuthorRole(message.Role),
            Content = message.Parts.First().Text
        };
    }

    public static Message ToMessage(this ChatMessageContent chatMessageContent)
    {
        Verify.NotNullOrWhiteSpace(chatMessageContent.Content);

        return new Message()
        {
            Role = chatMessageContent.Role.ToString(),
            Parts =
            [
                new TextPart
                {
                    Text = chatMessageContent.Content
                }
            ]
        };
    }
}

public static class AgentStateExtensions
{
    private const string SessionIdTag = "SessionId";
   

    public static void SetSessionId(this AgentState agentState, string sessionId)
    {
        agentState.Set(SessionIdTag, sessionId);
    }

    public static string GetSessionId(this AgentState agentState)
    {
        return agentState.GetOrDefault<string>(SessionIdTag);
    }
}