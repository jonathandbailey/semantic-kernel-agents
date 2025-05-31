using Todo.Core.Agents.Plugins;
using Todo.Core.Communication;

namespace Todo.Core.Agents.Build
{
    

    public class PluginFactory(IAgentStateStore agentStateStore, IAgentPublisher publisher) : IPluginFactory
    {
        public IAgentPlugin Create(string name, string agentName)
        {
            return name switch
            {
                "TaskPlugin" => new TaskPlugin(agentStateStore, agentName, publisher),
                _ => throw new InvalidOperationException($"Plugin not found: {name}")
            };
        }
    }

    public interface IAgentPlugin {}

    public interface IPluginFactory
    {
        IAgentPlugin Create(string name, string agentName);
    }
}
