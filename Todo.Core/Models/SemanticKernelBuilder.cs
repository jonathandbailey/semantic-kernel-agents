using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Todo.Core.Extensions;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Models;

#pragma warning disable SKEXP0070

public static class SemanticKernelBuilder
{
    public static Kernel CreateKernel(IConfiguration configuration)
    {
        Verify.NotNull(configuration);

        var modelSettings =
            configuration.GetRequiredSetting<List<LanguageModelSettings>>(SettingsConstants.LanguageModelSettings);

        var azureAiServiceSettings = configuration.GetRequiredSetting<AzureAiServiceSettings>();

        var kernelBuilder = Kernel.CreateBuilder();

        foreach (var settings in modelSettings)
        {
            switch (settings.Type)
            {
                case ModelTypes.AzureOpenAiChatCompletion:
                    AddAzureOpenAiChatCompletion(kernelBuilder, settings, azureAiServiceSettings);
                    break;

                case ModelTypes.AzureAiInferenceChatCompletion:
                    AddAzureAiInferenceChatCompletion(kernelBuilder, settings, azureAiServiceSettings);
                    break;

                default:
                    throw new NotSupportedException($"Model type '{settings.Type}' is not supported.");
            }
        }

        return kernelBuilder.Build();
    }

    private static void AddAzureOpenAiChatCompletion(IKernelBuilder kernelBuilder, LanguageModelSettings settings, AzureAiServiceSettings azureAiServiceSettings)
    {
        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: settings.DeploymentName,
            apiKey: azureAiServiceSettings.ApiKey,
            endpoint: azureAiServiceSettings.Endpoint,
            serviceId: settings.ServiceId
        );
    }

    private static void AddAzureAiInferenceChatCompletion(IKernelBuilder kernelBuilder, LanguageModelSettings settings, AzureAiServiceSettings azureAiServiceSettings)
    {
        kernelBuilder.AddAzureAIInferenceChatCompletion(
            modelId: settings.ModelName,
            apiKey: azureAiServiceSettings.ApiKey,
            endpoint: new Uri(azureAiServiceSettings.Endpoint),
            serviceId: settings.ServiceId
        );
    }
}
