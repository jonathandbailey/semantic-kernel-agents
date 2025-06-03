using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Infrastructure.File;

public class AgentTemplateFileRepository : IAgentTemplateRepository
{
    private readonly ILogger<AgentTemplateFileRepository> _logger;
    private readonly string _templateDirectory;

    public AgentTemplateFileRepository(IOptions<AzureStorageSettings> settings, ILogger<AgentTemplateFileRepository> logger)
    {
        _templateDirectory = settings.Value.AgentTemplatesContainerName;
        _logger = logger;
    }

    public async Task<string> GetAgentTemplateAsync(string agentTemplateName)
    {
        Verify.NotNullOrWhiteSpace(agentTemplateName);
            
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var myAppFolder = Path.Combine(documentsPath, _templateDirectory);

        var filePath = Path.Combine(myAppFolder, agentTemplateName);
            
            
        try
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"Agent template file not found: {filePath}");
            }
            using var reader = new StreamReader(filePath);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load agent template {agentTemplateName} from disk at {filePath}", agentTemplateName, filePath);
            throw;
        }
    }
}
