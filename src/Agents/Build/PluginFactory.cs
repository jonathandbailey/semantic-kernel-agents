using Agents.Plugins;
using Todo.Core.Vacations;

namespace Agents.Build
{
  
    public class PluginFactory(IVacationPlanService vacationPlanService) : IPluginFactory
    {
        public IAgentPlugin Create(string name, string agentName)
        {
            return name switch
            {
                "TravelTaskPlugin" => new TravelTaskPlugin(vacationPlanService),
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
