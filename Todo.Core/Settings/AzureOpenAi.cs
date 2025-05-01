using System.ComponentModel.DataAnnotations;

namespace Todo.Core.Settings;

public class AzureOpenAi
{
    [Required]
    public required string DeploymentName { get; set; }

    [Required]
    public required string ApiKey { get; set; }

    [Required]
    public required string Endpoint { get; set; }

    [Required]
    public required string Name { get; set; }
}