using Todo.Core.Agents;

namespace Todo.Core.Settings;

public class AgentSettings
{
    public AgentNames Name { get; set; } 

    public string Template { get; set; } = string.Empty;

    public string ServiceId { get; set; } = string.Empty;
}