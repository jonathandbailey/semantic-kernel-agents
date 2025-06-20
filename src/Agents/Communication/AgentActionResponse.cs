namespace Agents.Communication;

public class AgentActionResponse
{
    public required string Action { get; init; } = string.Empty;
    public required string Message { get; init; } = string.Empty;
}