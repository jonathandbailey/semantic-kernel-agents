using System.Diagnostics;
using Todo.Core.A2A;

namespace Todo.Agents.Middleware;

public class AgentTraceMiddleware(string agentName) : IAgentMiddleware
{
    private readonly ActivitySource _trace = new($"Todo.Agent.{agentName}");

    public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
    {
        using var activity = _trace.StartActivity($"{agentName}.{nameof(InvokeAsync)}");

        var agentTask = state.Get<AgentTask>("AgentTask");

        activity?.SetTag("SessionId", agentTask.SessionId);
        activity?.SetTag("Request", state.Request.Content);

        var response = await next(state);

        activity?.SetTag("Response", response.Responses.First().Content);

        return response;
    }
}