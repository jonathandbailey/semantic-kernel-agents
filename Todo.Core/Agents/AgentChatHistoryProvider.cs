using Microsoft.SemanticKernel.Agents;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Communication;
using Todo.Core.Infrastructure;

#pragma warning disable SKEXP0110

namespace Todo.Core.Agents
{
    public class AgentChatHistoryProvider(IChatHistoryRepository chatHistoryRepository) : IAgentChatHistoryProvider
    {
        public async Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string name)
        {
            var messages = await agentThread.GetMessagesAsync().ToListAsync();

            var convertedMessages = new List<Message>();

            foreach (var chatMessageContent in messages)
            {
                convertedMessages.Add(new Message
                {
                    Role = chatMessageContent.Role.ToString(),
                    Parts =
                    [
                        new TextPart
                        {
                            Text = chatMessageContent.Content!,
                        }
                    ]
                });
            }

            var json = JsonSerializer.Serialize(convertedMessages);

            await chatHistoryRepository.SaveChatHistoryAsync($"{name}", json);
        }

        public async Task<ChatHistoryAgentThread> LoadChatHistoryAsync(string name)
        {
            var json = await chatHistoryRepository.GetChatHistoryAsync(name);
            var messages = JsonSerializer.Deserialize<List<Message>>(json);
            var chatThread = new ChatHistoryAgentThread();
            
            if (messages != null)
                foreach (var message in messages)
                {
                    chatThread.ChatHistory.Add(new ChatMessageContent()
                    {
                        Role = new AuthorRole(message.Role),
                        Content = message.Parts.First().Text
                    });
                }

            return chatThread;
        }
    }

    public interface IAgentChatHistoryProvider
    {
        Task SaveChatHistoryAsync(ChatHistoryAgentThread agentThread, string name);
        Task<ChatHistoryAgentThread> LoadChatHistoryAsync(string name);
    }
}
