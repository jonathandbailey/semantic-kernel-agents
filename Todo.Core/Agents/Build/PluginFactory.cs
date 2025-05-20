using Todo.Core.Agents.Plugins;

namespace Todo.Core.Agents.Build
{
    

    public class PluginFactory : IPluginFactory
    {
        public IAgentPlugin Create(string name, IAgentProvider agentProvider)
        {
            return name switch
            {
                "TaskPlugin" => new TaskPlugin(agentProvider),
                _ => throw new InvalidOperationException($"Plugin not found: {name}")
            };
        }
    }

    public interface IAgentPlugin {}

    public interface IPluginFactory
    {
        IAgentPlugin Create(string name, IAgentProvider agentProvider);
    }
}
