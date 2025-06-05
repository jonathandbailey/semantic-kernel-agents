using System.Diagnostics;

namespace Todo.Application.Agents.Middleware;

public class AgentTraceMiddleware(string agentName) : IAgentMiddleware
{
    private readonly ActivitySource _trace = new($"Todo.Agent.{agentName}");

    public async Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
    {
        using var activity = _trace.StartActivity($"{agentName}.{nameof(InvokeAsync)}");

        activity?.SetTag("SessionId", context.SessionId);
        activity?.SetTag("Request", context.ChatCompletionRequest.Message);

        var response = await next(context);

        activity?.SetTag("Response", response.ChatCompletionResponse?.Message);

        return response;
    }
}