using System.Text;
using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Todo.Core.A2A;
using Todo.Infrastructure.Settings;

namespace Todo.Infrastructure.Azure;

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
    
    public async Task SaveChatHistoryAsync(string name, List<Message> messages)
    {
        Verify.NotNullOrWhiteSpace(name);

        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(name);

            var json = JsonSerializer.Serialize(messages);

            Verify.NotNullOrWhiteSpace(json);

            using var stream = new MemoryStream();
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await blobClient.UploadAsync(memoryStream, overwrite: true);
        }
        catch (RequestFailedException requestFailedException)
        {
            _logger.LogError(requestFailedException,
                "Azure Request Failed to save chat history to blob storage container {containerName} : {errorCode}",
                _blobContainerClient.Name, requestFailedException.ErrorCode);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,
                "Unknown Exception trying to save chat history to blob storage container {containerName}",
                _blobContainerClient.Name);
            throw;
        }
    }

    public async Task<List<Message>> GetChatHistoryAsync(string name)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(name);
            var exists = (await blobClient.ExistsAsync()).Value;

            if (!exists)
            {
                return [];
            }

            var blob = await _blobContainerClient.DownloadBlobAsync(name);

            Verify.NotNullOrWhiteSpace(blob);

            var messages = JsonSerializer.Deserialize<List<Message>>(blob);
                
            Verify.NotNull(messages);

            return messages;
        }
        catch (RequestFailedException requestFailedException)
        {
            _logger.LogError(requestFailedException,
                "Azure Request Failed to get chat history {name} from blob storage container {containerName} : {errorCode}",
                name, _blobContainerClient.Name, requestFailedException.ErrorCode);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,
                "Unknown Exception trying to get chat history {name} from blob storage container {containerName}",
                name, _blobContainerClient.Name);
            throw;
        }
    }
}

public interface IChatHistoryRepository
{
    Task<List<Message>> GetChatHistoryAsync(string name);
    Task SaveChatHistoryAsync(string name, List<Message> messages);
}