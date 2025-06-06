using Todo.Core.A2A;

namespace Todo.Agents
{
    public class AgentState
    {
        public required string SessionId { get; set; }

        public required string TaskId { get; set; }

        public required ChatCompletionRequest ChatCompletionRequest { get; set; }

        public ChatCompletionResponse? ChatCompletionResponse { get; set; }

        public required AgentTask AgentTask { get; init; }
    }
}
