using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Todo.Infrastructure.Azure;

namespace Todo.Infrastructure.File;

public class AgentTemplateFileRepository : IAgentTemplateRepository
{
    private readonly ILogger<AgentTemplateFileRepository> _logger;
    private readonly string _directoryPath;

    public AgentTemplateFileRepository(IOptions<FileStorageSettings> settings, ILogger<AgentTemplateFileRepository> logger)
    {
        _logger = logger;

        _directoryPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{settings.Value.ApplicationName}\{settings.Value.TemplateFolder}";
    }

    public async Task<string> GetAgentTemplateAsync(string agentTemplateName)
    {
        Verify.NotNullOrWhiteSpace(agentTemplateName);

        var filePath = Path.Combine(_directoryPath, agentTemplateName);

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
