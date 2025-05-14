using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using Todo.Core.Extensions;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Infrastructure
{
    public interface IChatHistoryRepository
    {
        Task<string> GetChatHistoryAsync(string chatSessionId);
        Task SaveChatHistoryAsync(string chatSessionId, string json);
    }

    public class ChatHistoryRepository : IChatHistoryRepository
    {
        private readonly ILogger<ChatHistoryRepository> _logger;
        private readonly BlobContainerClient _blobContainerClient;

        public ChatHistoryRepository(IOptions<AzureStorageSettings> settings, ILogger<ChatHistoryRepository> logger)
        {
            _logger = logger;
            var blobServiceClient = new BlobServiceClient(settings.Value.ConnectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.Value.ChatHistoryContainerName);
        }

        public async Task SaveChatHistoryAsync(string chatSessionId, string json)
        {
            Verify.NotNullOrWhiteSpace(json);
         
            try
            {
                //create the blob if it doesn't exist
                await _blobContainerClient.CreateIfNotExistsAsync();
                
                var blobClient = _blobContainerClient.GetBlobClient(chatSessionId);

                using var stream = new MemoryStream();
                using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                await blobClient.UploadAsync(memoryStream, overwrite: true);
            }
            catch (RequestFailedException requestFailedException)
            {
                _logger.LogError(requestFailedException,
                    "Azure Request Failed to save chat history {json} to blob storage container {containerName} : {errorCode}",
                    json, _blobContainerClient.Name, requestFailedException.ErrorCode);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Unknown Exception trying to save chat history {json} to blob storage container {containerName}",
                    json, _blobContainerClient.Name);
                throw;
            }
        }

        public async Task<string> GetChatHistoryAsync(string chatSessionId)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(chatSessionId);
                var exists = (await blobClient.ExistsAsync()).Value;

                if (!exists)
                {
                    return "[]";
                }

                return await _blobContainerClient.DownloadBlobAsync(chatSessionId);
            }
            catch (RequestFailedException requestFailedException)
            {
                _logger.LogError(requestFailedException,
                    "Azure Request Failed to get chat history {chatSessionId} from blob storage container {containerName} : {errorCode}",
                    chatSessionId, _blobContainerClient.Name, requestFailedException.ErrorCode);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Unknown Exception trying to get chat history {chatSessionId} from blob storage container {containerName}",
                    chatSessionId, _blobContainerClient.Name);
                throw;
            }
        }
    }
}
