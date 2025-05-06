using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Core.Agents;
using Todo.Core.Infrastructure;
using Todo.Core.Models;
using Todo.Core.Services;

namespace Todo.Core.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAgentTemplateRepository, AgentTemplateRepository>();
            services.AddSingleton<IAgentConfigurationProvider, AgentConfigurationProvider>();

            services.AddSingleton(SemanticKernelBuilder.CreateKernel(configuration));

            services.AddAgents();

            services.AddSingleton<ITodoService, TodoService>();
        }
    }
}
