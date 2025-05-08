using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;
using Todo.Core.Infrastructure;
using Todo.Core.Messaging;
using Todo.Core.Models;
using Todo.Core.Services;
using Todo.Core.Settings;
using Todo.Core.Users;

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

            services.AddScoped<IAgentTemplateRepository, AgentTemplateRepository>();
            services.AddScoped<IAgentConfigurationProvider, AgentConfigurationProvider>();


            services.AddScoped<IAgentProvider, AgentProvider>();

            services.AddScoped<IUser, User>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAgent).Assembly));

            services.AddScoped<IMessagePublisher, MessagePublisher>();

            services.AddSingleton(SemanticKernelBuilder.CreateKernel(configuration));

            services.AddAgents();

            services.AddScoped<ITodoService, TodoService>();
        }
    }
}
