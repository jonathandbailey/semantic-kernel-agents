using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Todo.Core.Extensions;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core.Models;

#pragma warning disable SKEXP0070

public static class SemanticKernelBuilder
{
    public static Kernel CreateKernel(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        Verify.NotNull(configuration);

        var modelSettings =
            configuration.GetRequiredSetting<List<LanguageModelSettings>>(SettingsConstants.LanguageModelSettings);
       
        var kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.Services.AddSingleton(loggerFactory);

        foreach (var settings in modelSettings)
        {
            switch (settings.Type)
            {
                case ModelTypes.AzureOpenAiChatCompletion:
                    AddAzureOpenAiChatCompletion(kernelBuilder, settings);
                    break;

                case ModelTypes.AzureAiInferenceChatCompletion:
                    AddAzureAiInferenceChatCompletion(kernelBuilder, settings);
                    break;

                default:
                    throw new NotSupportedException($"Model type '{settings.Type}' is not supported.");
            }
        }

        return kernelBuilder.Build();
    }

    private static void AddAzureOpenAiChatCompletion(IKernelBuilder kernelBuilder, LanguageModelSettings settings)
    {
        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: settings.DeploymentName,
            apiKey: settings.ApiKey,
            endpoint: settings.Endpoint,
            serviceId: settings.ServiceId
        );
    }

    private static void AddAzureAiInferenceChatCompletion(IKernelBuilder kernelBuilder, LanguageModelSettings settings)
    {
        kernelBuilder.AddAzureAIInferenceChatCompletion(
            modelId: settings.ModelName,
            apiKey: settings.ApiKey,
            endpoint: new Uri(settings.Endpoint),
            serviceId: settings.ServiceId
        );
    }
}
