using System.Text;
using Microsoft.SemanticKernel.Agents;
using Todo.Core.A2A;
using Todo.Infrastructure.Azure;

#pragma warning disable SKEXP0110

namespace Agents;

public class AgentChatHistoryProvider(IChatHistoryRepository chatHistoryRepository) : IAgentChatHistoryProvider
{
    public async Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string agentName, string sessionId)
    {
        var messages = await agentThread.GetMessagesAsync().ToListAsync();

        var convertedMessages = new List<Message>();

        foreach (var chatMessageContent in messages)
        {
            if (!string.IsNullOrEmpty(chatMessageContent.Content))
            {
                convertedMessages.Add(chatMessageContent.ToMessage());
            }
        }

        await chatHistoryRepository.SaveChatHistoryAsync(GetFileName(sessionId, agentName), convertedMessages);
    }

    public async Task<ChatHistoryAgentThread> LoadChatHistoryAsync(string agentName, string sessionId)
    {
        var messages = await chatHistoryRepository.GetChatHistoryAsync(GetFileName(sessionId, agentName));
        
        var chatThread = new ChatHistoryAgentThread();
       
        foreach (var message in messages)
        {
            chatThread.ChatHistory.Add(message.ToChatMessageContent());
        }

        return chatThread;
    }

    public async Task AddToConversation(string sessionId, ChatHistoryAgentThread agentThread, string agentName)
    {
        var newestMessages = agentThread.ChatHistory.TakeLast(2).ToList();

        if (!newestMessages.Any())
        {
            return;
        }

        var filename = GetConversationFileName(sessionId);

        var conversation = await chatHistoryRepository.GetConversationAsync(filename);

        var stringBuilder = new StringBuilder(conversation);

        foreach (var chatMessageContent in newestMessages)
        {
            var content = AgentHeaderParser.RemoveHeaders(chatMessageContent.Content!);
            
            stringBuilder.AppendLine($"**[{agentName}]-[{chatMessageContent.Role}]** : {content}");
            stringBuilder.AppendLine();
        }

        await chatHistoryRepository.SaveConversationAsync(filename, stringBuilder.ToString());
    }

    private static string GetFileName(string sessionId, string agentName)
    {
        return $"{sessionId} - [{agentName}].json";
    }

    private static string GetConversationFileName(string sessionId)
    {
        return $"{sessionId} - conversation.md";
    }
}

public interface IAgentChatHistoryProvider
{
    Task<ChatHistoryAgentThread> LoadChatHistoryAsync(string agentName, string sessionId);
    Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string agentName, string sessionId);
    Task AddToConversation(string sessionId, ChatHistoryAgentThread agentThread, string agentName);
}