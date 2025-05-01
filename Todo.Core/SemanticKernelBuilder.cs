using Microsoft.SemanticKernel;
using Todo.Core.Settings;

namespace Todo.Core
{
    public static class SemanticKernelBuilder
    {
        public static Kernel CreateKernel(LanguageModelSettings modelSettings)
        {
            var kernelBuilder = Kernel.CreateBuilder();

            foreach (var azureOpenAiSettings in modelSettings.AzureOpenAiSettings)
            {
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    deploymentName: azureOpenAiSettings.DeploymentName,
                    apiKey: azureOpenAiSettings.ApiKey,
                    endpoint: azureOpenAiSettings.Endpoint,
                    serviceId: azureOpenAiSettings.Name
                );
            }

            return kernelBuilder.Build();
        }
    }
}
