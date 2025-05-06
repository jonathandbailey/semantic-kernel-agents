using Todo.Core.Models;

namespace Todo.Core.Settings;

public class LanguageModelSettings
{
    public string ModelName { get; set; } = string.Empty;

    public string DeploymentName { get; set; } = string.Empty;

    public string ServiceId { get; set; } = string.Empty;

    public ModelTypes Type { get; set; }

    public string Endpoint { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}

