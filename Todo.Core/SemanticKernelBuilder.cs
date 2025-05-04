using Microsoft.SemanticKernel;
using Todo.Core.Settings;
using Todo.Core.Utilities;

namespace Todo.Core;

public static class SemanticKernelBuilder
{
    public static Kernel CreateKernel(List<LanguageModelSettings> modelSettings)
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
                        apiKey: settings.ApiKey,
                        endpoint: settings.Endpoint,
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
