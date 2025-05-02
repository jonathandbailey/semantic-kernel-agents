namespace Todo.Core.Settings;

public class AzureStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;

    public string AgentTemplatesContainerName { get; set;  } = string.Empty;
}