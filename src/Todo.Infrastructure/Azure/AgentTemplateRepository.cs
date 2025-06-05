using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Todo.Infrastructure.Azure;

public class AgentTemplateRepository : IAgentTemplateRepository
{
    private readonly ILogger<AgentTemplateRepository> _logger;
    private readonly BlobContainerClient _blobContainerClient;

    public AgentTemplateRepository(IOptions<AzureStorageSettings> settings, ILogger<AgentTemplateRepository> logger)
    {
        _logger = logger;

        var blobServiceClient = new BlobServiceClient(settings.Value.ConnectionString);
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(settings.Value.AgentTemplatesContainerName);
    }

    public async Task<string> GetAgentTemplateAsync(string agentTemplateName)
    {
        Verify.NotNullOrWhiteSpace(agentTemplateName);
        
        try
        {
            return await _blobContainerClient.DownloadBlobAsync(agentTemplateName);
        }
        catch (RequestFailedException requestFailedException)
        {
            _logger.LogError(requestFailedException,
                "Azure Request Failed to get agent template {agentTemplateName} from blob storage container {containerName} : {errorCode}", agentTemplateName, _blobContainerClient.Name, requestFailedException.ErrorCode);
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,
                "Unknown Exception trying to get agent template {agentTemplateName} from blob storage container {containerName}", agentTemplateName, _blobContainerClient.Name);
            throw;
        }
    }
}

public interface IAgentTemplateRepository
{
    Task<string> GetAgentTemplateAsync(string agentTemplateName);
}