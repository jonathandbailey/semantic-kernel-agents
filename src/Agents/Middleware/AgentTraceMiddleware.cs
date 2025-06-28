using System.Diagnostics;

namespace Agents.Middleware;

public class AgentTraceMiddleware(string agentName) : IAgentMiddleware
{
    private readonly ActivitySource _trace = new($"Todo.Agent.{agentName}");

    public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
    {
        using var activity = _trace.StartActivity($"{agentName}.{nameof(InvokeAsync)}");
    
        activity?.SetTag("SessionId", state.SessionId);
        activity?.SetTag("Request", state.Request.Content);

        var response = await next(state);

        activity?.SetTag("Response", response.Response.Content);

        return response;
    }
}