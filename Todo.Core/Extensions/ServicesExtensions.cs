using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;
using Todo.Core.Infrastructure;
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
         
            services.AddScoped<IAgentProvider, AgentProvider>();

            services.AddScoped<IAgentDiscovery, AgentDiscovery>();

            services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();
          
            services.AddScoped<IUser, User>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAgent).Assembly));

            services.AddScoped( _=> SemanticKernelBuilder.CreateKernel(configuration));

            services.AddScoped<ITodoService, TodoService>();
        }
    }
}
