using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Todo.Core.A2A;
using Todo.Infrastructure.Azure;

namespace Todo.Infrastructure.File
{
    public class ChatHistoryFileRepository : IChatHistoryRepository
    {
        private readonly ILogger<ChatHistoryFileRepository> _logger;
        private readonly string _directoryPath;

        public ChatHistoryFileRepository(IOptions<FileStorageSettings> settings, ILogger<ChatHistoryFileRepository> logger)
        {
            _logger = logger;
         
            _directoryPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{settings.Value.ApplicationName}\{settings.Value.ChatHistoryFolder}";
        }

        public async Task SaveChatHistoryAsync(string name, List<Message> messages)
        {
            Verify.NotNullOrWhiteSpace(name);
            
            try
            {
                var json = JsonSerializer.Serialize(messages);

                var filePath = Path.Combine(_directoryPath, name);

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
                var filePath = Path.Combine(_directoryPath, name);

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
