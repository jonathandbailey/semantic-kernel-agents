namespace Todo.Agents.Build
{
  
    public class PluginFactory : IPluginFactory
    {
        public IAgentPlugin Create(string name, string agentName)
        {
            return name switch
            {
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
