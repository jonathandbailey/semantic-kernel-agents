using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Todo.Agents;
using Todo.Agents.Build;
using Todo.Agents.Settings;
using Todo.Application.Models;
using Todo.Application.Services;
using Todo.Application.Settings;
using Todo.Core.Users;
using Todo.Infrastructure;
using Todo.Infrastructure.Azure;
using Todo.Infrastructure.File;

namespace Todo.Application.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {


            services.Configure<FileStorageSettings>(options =>
                configuration.GetSection(SettingsConstants.FileStorageSettings).Bind(options));

            services.Configure<AzureStorageSettings>(options =>
                configuration.GetSection(SettingsConstants.AzureStorageSettings).Bind(options));

            services.Configure<List<AgentSettings>>(options =>
                configuration.GetSection(SettingsConstants.AgentSettings).Bind(options));

            services.AddScoped<IAgentTemplateRepository, AgentTemplateFileRepository>();
         
            services.AddScoped<IAgentProvider, AgentProvider>();
        
            services.AddScoped<IAgentFactory, AgentFactory>();
            services.AddScoped<IPluginFactory, PluginFactory>();

            services.AddScoped<IAgentTaskRepository, AgentTaskFileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
     
            services.AddScoped<IChatHistoryRepository, ChatHistoryFileRepository>();
            services.AddScoped<IAgentChatHistoryProvider, AgentChatHistoryProvider>();

            services.AddScoped<IUser, User>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(IAgent).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(TodoService).Assembly);
            });

            services.AddScoped( _=> SemanticKernelBuilder.CreateKernel(configuration, loggerFactory));

            services.AddScoped<ITodoService, TodoService>();
        }
    }
}
