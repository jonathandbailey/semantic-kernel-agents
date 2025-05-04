using Microsoft.SemanticKernel;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Models;

public static class SemanticKernelBuilder
{
    public static Kernel CreateKernel(List<LanguageModelSettings> modelSettings, AzureAiServiceSettings azureAiServiceSettings)
    {
        Verify.NotNull(modelSettings);

        var kernelBuilder = Kernel.CreateBuilder();

        foreach (var settings in modelSettings)
        {
            switch (settings.Type)
            {
                case ModelTypes.AzureOpenAiChatCompletion:
                    kernelBuilder.AddAzureOpenAIChatCompletion(
                        deploymentName: settings.DeploymentName,
                        apiKey: azureAiServiceSettings.ApiKey,
                        endpoint: azureAiServiceSettings.Endpoint,
                        serviceId: settings.ServiceId
                    );
                    break;
#pragma warning disable SKEXP0070
                case ModelTypes.AzureAiInferenceChatCompletion:
                    kernelBuilder.AddAzureAIInferenceChatCompletion(
                        modelId: settings.ModelName,
                        apiKey: azureAiServiceSettings.ApiKey,
                        endpoint: new Uri(azureAiServiceSettings.Endpoint),
                        serviceId: settings.ServiceId
                    );
                    break;

                default:
                    throw new NotSupportedException($"Model type '{settings.Type}' is not supported.");
            }
        }

        return kernelBuilder.Build();
    }
}
