using Todo.Core.Agents.Plugins;

namespace Todo.Core.Agents.Build
{
    

    public class PluginFactory(IAgentStateStore agentStateStore) : IPluginFactory
    {
        public IAgentPlugin Create(string name, IAgentProvider agentProvider, string agentName)
        {
            return name switch
            {
                "TaskPlugin" => new TaskPlugin(agentProvider, agentStateStore, agentName),
                _ => throw new InvalidOperationException($"Plugin not found: {name}")
            };
        }
    }

    public interface IAgentPlugin {}

    public interface IPluginFactory
    {
        IAgentPlugin Create(string name, IAgentProvider agentProvider, string agentName);
    }
}
