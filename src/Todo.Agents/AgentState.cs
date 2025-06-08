using Microsoft.SemanticKernel;
using Todo.Core.A2A;

namespace Todo.Agents
{
    public class AgentState
    {
        public required ChatMessageContent Request { get; init; }

        public List<ChatMessageContent> Responses { get; set; } = [];
    
        public required AgentTask AgentTask { get; init; }

        private Dictionary<string, object> Data { get; } = new();

        public T Get<T>(string key)
        {
            if (!Data.TryGetValue(key, out var val))
                throw new KeyNotFoundException($"Key '{key}' was not found in the AgentState data.");
            return (T)val;
        }
        
        public void Set<T>(string key, T value) => Data[key] = value!;
    }
}
