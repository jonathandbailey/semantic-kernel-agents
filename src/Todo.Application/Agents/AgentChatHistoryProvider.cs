using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Application.Extensions;
using Todo.Core.A2A;
using Todo.Infrastructure.Azure;

#pragma warning disable SKEXP0110

namespace Todo.Application.Agents;

public class AgentChatHistoryProvider(IChatHistoryRepository chatHistoryRepository) : IAgentChatHistoryProvider
{
    public async Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string agentName, string sessionId)
    {
        var messages = await agentThread.GetMessagesAsync().ToListAsync();

        var convertedMessages = new List<Message>();

        foreach (var chatMessageContent in messages)
        {
            if (!string.IsNullOrEmpty(chatMessageContent.Content) && chatMessageContent.Role != AuthorRole.Tool)
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

    private static string GetFileName(string sessionId, string agentName)
    {
        return $"{sessionId} - [{agentName}].json";
    }
}

public interface IAgentChatHistoryProvider
{
    Task<ChatHistoryAgentThread> LoadChatHistoryAsync(string agentName, string sessionId);
    Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string agentName, string sessionId);
}