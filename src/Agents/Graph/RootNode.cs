namespace Agents.Graph
{
    public class RootNode(string name) : INode
    {
        public Task<AgentState> InvokeAsync(AgentState state)
        {
            var source = state.Get<string>("source");

            if (string.IsNullOrEmpty(source))
            {
                state.Set("source", AgentNames.UserAgent);
            }
            
            return Task.FromResult(state);
        }

        public string Name { get; } = name; 
    }
}
