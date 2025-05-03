using Microsoft.SemanticKernel;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core;

public static class SemanticKernelBuilder
{
    public static Kernel CreateKernel(LanguageModelSettings modelSettings)
    {
        Verify.NotNull(modelSettings);
            
        var kernelBuilder = Kernel.CreateBuilder();

        foreach (var azureOpenAiSettings in modelSettings.AzureOpenAiSettings)
        {
            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: azureOpenAiSettings.DeploymentName,
                apiKey: azureOpenAiSettings.ApiKey,
                endpoint: azureOpenAiSettings.Endpoint,
                serviceId: azureOpenAiSettings.ServiceId
            );
        }

        return kernelBuilder.Build();
    }
}