namespace Todo.Infrastructure.Settings;

public class AzureStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;

    public string AgentTemplatesContainerName { get; set;  } = string.Empty;

    public string ChatHistoryContainerName { get; set; } = string.Empty;

    public string AgentTaskContainerName { get; set; } = string.Empty;
}