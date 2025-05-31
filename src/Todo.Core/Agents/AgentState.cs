using Todo.Core.Communication;

namespace Todo.Core.Agents
{
    public class AgentState
    {
        public required string SessionId { get; set; }

        public required string TaskId { get; set; }

        public required ChatCompletionRequest ChatCompletionRequest { get; set; }

        public ChatCompletionResponse? ChatCompletionResponse { get; set; }
    }
}
