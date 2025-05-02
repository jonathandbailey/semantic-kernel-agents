namespace Todo.Core.Settings;

public class AzureOpenAiSettings
{
    public string DeploymentName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string ServiceId { get; set; } = string.Empty;
}