using System.Diagnostics;
using Todo.Core.Communication;

namespace Todo.Core.Agents.Middleware;

public class AgentTraceMiddleware(string agentName) : IAgentMiddleware
{
    private readonly ActivitySource _trace = new($"Todo.Agent.{agentName}");

    public async Task<ChatCompletionResponse> InvokeAsync(ChatCompletionRequest context, AgentDelegate next)
    {
        using var activity = _trace.StartActivity($"{agentName}.{nameof(InvokeAsync)}");

        activity?.SetTag("SessionId", context.SessionId);
        activity?.SetTag("Request", context.Message);

        var response = await next(context);

        activity?.SetTag("Response", response.Message);

        return response;
    }
}