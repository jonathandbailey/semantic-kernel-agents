using Microsoft.SemanticKernel;

namespace Agents
{
    public class AgentState(string agentName)
    {
        public string AgentName { get; } = agentName;

        public required ChatMessageContent Request { get; set; }

        public ChatMessageContent Response { get; set; } = new ChatMessageContent();

        public Guid SessionId { get; set; } = Guid.Empty;

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

        public bool TryGet<T>(string key, out T value)
        {
            if (Data.TryGetValue(key, out var val) && val is T tVal)
            {
                value = tVal;
                return true;
            }
            value = default!;
            return false;
        }

        public T GetOrDefault<T>(string key)
        {
            if (Data.TryGetValue(key, out var val) && val is T tVal)
            {
                
                return tVal;
            }
            
            return default!;
        }

        public void Set<T>(string key, T value) => Data[key] = value!;
    }
}
