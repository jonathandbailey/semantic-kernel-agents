using Todo.Application.Agents.A2A;

namespace Todo.Application.Users
{
    public class UserResponse
    {
        public string SessionId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public required AgentTask Task { get; set; }
    }
}
