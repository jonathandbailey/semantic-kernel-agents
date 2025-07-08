using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Agents
{
    public class AgentState(string agentName)
    {
        public string AgentName { get; } = agentName;

        public required ChatMessageContent Request { get; set; }

        public ChatMessageContent Response { get; set; } = new ChatMessageContent();

        public Guid SessionId { get; set; } = Guid.Empty;

        public ChatHistoryAgentThread AgentThread { get; set; } = new ChatHistoryAgentThread();

        public Dictionary<string, string> Arguments { get; set; } = [];

        private Dictionary<string, object> Data { get; } = new();

        public T Get<T>(string key)
        {
            if (!Data.TryGetValue(key, out var val))
                throw new KeyNotFoundException($"Key '{key}' was not found in the AgentState data.");
            return (T)val;
        }

        public bool HasKey(string key)
        {
            return Data.ContainsKey(key);
        }
      

        public void Set<T>(string key, T value) => Data[key] = value!;
    }
}
