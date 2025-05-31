namespace Todo.Core.Agents.Middleware;

public class AgentMiddlewareBuilder
{
    private readonly List<Func<AgentDelegate, AgentDelegate>> _middlewares = [];

    public AgentMiddlewareBuilder Use(IAgentMiddleware middleware)
    {
        _middlewares.Add(next =>
            context => middleware.InvokeAsync(context, next));
        return this;
    }

    public AgentDelegate Build()
    {
        AgentDelegate app = _ => throw new InvalidOperationException("No terminal handler registered.");

        foreach (var component in _middlewares.AsEnumerable().Reverse())
        {
            app = component(app);
        }

        return app;
    }
}

public interface IAgentMiddleware
{
    Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next);
}


public delegate Task<AgentState> AgentDelegate(AgentState request);