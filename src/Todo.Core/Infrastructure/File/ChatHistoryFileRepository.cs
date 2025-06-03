using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Todo.Core.Agents.A2A;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Infrastructure.File
{
    public class ChatHistoryFileRepository : IChatHistoryRepository
    {
        private readonly ILogger<ChatHistoryFileRepository> _logger;
        private readonly string _directoryPath;

        public ChatHistoryFileRepository(IOptions<AzureStorageSettings> settings, ILogger<ChatHistoryFileRepository> logger)
        {
            _logger = logger;
            _directoryPath = settings.Value.ChatHistoryContainerName;
            
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        public async Task SaveChatHistoryAsync(string name, List<Message> messages)
        {
            Verify.NotNullOrWhiteSpace(name);
            
            try
            {
                var json = JsonSerializer.Serialize(messages);

                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var myAppFolder = Path.Combine(documentsPath, _directoryPath);

                var filePath = Path.Combine(myAppFolder, name);

                Verify.NotNullOrWhiteSpace(json);
                await System.IO.File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Exception trying to save chat history to file {filePath}",
                    name);
                throw;
            }
        }

        public async Task<List<Message>> GetChatHistoryAsync(string name)
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var myAppFolder = Path.Combine(documentsPath, _directoryPath);

                var filePath = Path.Combine(myAppFolder, name);
                
                if (!System.IO.File.Exists(filePath))
                {
                    return [];
                }
                
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                
                Verify.NotNullOrWhiteSpace(json);
                
                var messages = JsonSerializer.Deserialize<List<Message>>(json);
                
                Verify.NotNull(messages);
                
                return messages;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Exception trying to get chat history from file {filePath}",
                    name);
                throw;
            }
        }
    }
}
