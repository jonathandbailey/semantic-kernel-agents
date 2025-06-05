namespace Todo.Application.Agents.A2A
{
    public class AgentTaskStatus
    {
        public string State { get; set;  } = string.Empty;

        public Message Message { get; set; } = new Message();
    }
}
