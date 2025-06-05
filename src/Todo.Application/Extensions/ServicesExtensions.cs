using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Todo.Application.Agents;
using Todo.Application.Agents.Build;
using Todo.Application.Communication;
using Todo.Application.Infrastructure;
using Todo.Application.Infrastructure.File;
using Todo.Application.Models;
using Todo.Application.Services;
using Todo.Application.Settings;
using Todo.Core.Users;

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

            services.AddScoped<IAgentStateStore, AgentStateStore>();

            services.AddScoped<IAgentFactory, AgentFactory>();
            services.AddScoped<IPluginFactory, PluginFactory>();

            services.AddScoped<IAgentTaskRepository, AgentTaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IAgentPublisher, AgentPublisher>();

            services.AddScoped<IChatHistoryRepository, ChatHistoryFileRepository>();
            services.AddScoped<IAgentChatHistoryProvider, AgentChatHistoryProvider>();

            services.AddScoped<IUser, User>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAgent).Assembly));

            services.AddScoped( _=> SemanticKernelBuilder.CreateKernel(configuration, loggerFactory));

            services.AddScoped<ITodoService, TodoService>();
        }
    }
}
