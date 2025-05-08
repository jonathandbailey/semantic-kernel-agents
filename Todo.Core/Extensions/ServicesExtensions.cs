using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;
using Todo.Core.Infrastructure;
using Todo.Core.Models;
using Todo.Core.Services;
using Todo.Core.Settings;

namespace Todo.Core.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureStorageSettings>(options =>
                configuration.GetSection(SettingsConstants.AzureStorageSettings).Bind(options));

            services.Configure<List<AgentSettings>>(options =>
                configuration.GetSection(SettingsConstants.AgentSettings).Bind(options));

            services.AddSingleton<IAgentTemplateRepository, AgentTemplateRepository>();
            services.AddSingleton<IAgentConfigurationProvider, AgentConfigurationProvider>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAgent).Assembly));

            services.AddSingleton(SemanticKernelBuilder.CreateKernel(configuration));

            services.AddAgents();

            services.AddSingleton<ITodoService, TodoService>();
        }
    }
}
