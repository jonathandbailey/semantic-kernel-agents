using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Todo.Core.Agents.A2A;
using Todo.Core.Extensions;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Infrastructure
{
    public class AgentTaskRepository : IAgentTaskRepository
    {
        private readonly ILogger<ChatHistoryRepository> _logger;
        private readonly BlobContainerClient _blobContainerClient;

        public AgentTaskRepository(IOptions<AzureStorageSettings> settings, ILogger<ChatHistoryRepository> logger)
        {
            _logger = logger;
            var blobServiceClient = new BlobServiceClient(settings.Value.ConnectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.Value.AgentTaskContainerName);
        }

        public async Task<bool> Exists(string taskId)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient($"{taskId}.json");
                var exists = (await blobClient.ExistsAsync()).Value;

                return exists;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to get verify if {taskId} exists from blob storage.");
                throw;
            }

        }

        public async Task<AgentTask> LoadAsync(string taskId)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient($"{taskId}.json");
                var exists = (await blobClient.ExistsAsync()).Value;

                if(!exists)
                {
                    throw new AgentException($"Blob with name {taskId} does not exist.");
                }

                var blob = await _blobContainerClient.DownloadBlobAsync($"{taskId}.json");

                Verify.NotNullOrWhiteSpace(blob);

                var agentTask = JsonSerializer.Deserialize<AgentTask>(blob);

                Verify.NotNull(agentTask);

                return agentTask;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to get agent task with ID {taskId} from blob storage.");
                throw;
            }

        }

        public async Task SaveAsync(AgentTask agentTask)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient($"{agentTask.TaskId}.json");

                var json = JsonSerializer.Serialize(agentTask);

                Verify.NotNullOrWhiteSpace(json);

                using var stream = new MemoryStream();
                using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                await blobClient.UploadAsync(memoryStream, overwrite: true);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to save agent task with ID {agentTask.TaskId} to blob storage.", agentTask.TaskId);
                throw;
            }
        }
    }

    public interface IAgentTaskRepository
    {
        Task<AgentTask> LoadAsync(string taskId);
        Task SaveAsync(AgentTask agentTask);
        Task<bool> Exists(string taskId);
    }
}
