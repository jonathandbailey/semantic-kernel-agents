namespace Agents.Graph
{
    public class NodeState(AgentState agentState)
    {
        public AgentState AgentState { get; } = agentState;

        public string Source { get; set; } = string.Empty;

        public string Headers { get; set; } = string.Empty;
    }
}
