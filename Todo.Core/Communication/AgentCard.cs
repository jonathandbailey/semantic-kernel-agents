namespace Todo.Core.Communication
{
    public class AgentCard
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<AgentSkill> Skills { get; set; } = [];
    }
}
