using Microsoft.SemanticKernel.Agents;
using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Extensions;
using Todo.Core.Infrastructure;
using Todo.Core.Agents.A2A;

#pragma warning disable SKEXP0110

namespace Todo.Core.Agents;

public class AgentChatHistoryProvider(IChatHistoryRepository chatHistoryRepository) : IAgentChatHistoryProvider
{
    public async Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string name)
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

        var json = JsonSerializer.Serialize(convertedMessages);

        // TODO : Chat History Repository should take a list of messages and save them in a single blob
        await chatHistoryRepository.SaveChatHistoryAsync($"{name}.json", json);
    }

    public async Task<ChatHistoryAgentThread> LoadChatHistoryAsync(string name)
    {
        //TODO : Chat History Should return a list of messages
        var json = await chatHistoryRepository.GetChatHistoryAsync($"{name}.json");
        var messages = JsonSerializer.Deserialize<List<Message>>(json);
        var chatThread = new ChatHistoryAgentThread();
            
        if (messages != null)
            foreach (var message in messages)
            {
                chatThread.ChatHistory.Add(message.ToChatMessageContent());
            }

        return chatThread;
    }
}

public interface IAgentChatHistoryProvider
{
    Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string name);
    Task<ChatHistoryAgentThread> LoadChatHistoryAsync(string name);
}