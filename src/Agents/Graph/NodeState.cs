namespace Agents.Graph
{
    public class NodeState(AgentState agentState)
    {
        public AgentState AgentState { get; } = agentState;

        public string Source { get; set; } = string.Empty;

        public string Headers { get; set; } = string.Empty;

        public Guid VacationPlanId { get; set; } = Guid.Empty;

        public Guid Routing { get; set; } = Guid.Empty;

        public string Route { get; set; } = string.Empty;
    }
}
