namespace Agents.Middleware;

public class TaskParserMiddleware : IAgentMiddleware
{
    public async Task<AgentState> InvokeAsync(AgentState state, AgentDelegate next)
    {
        var response = await next(state);
        
        var headers = AgentHeaderParser.ExtractHeaders(response.Response.Content!);

        var headerValues = AgentHeaderParser.ExtractHeaderValues(headers, "task-id");

        if (headerValues.ContainsKey("task-id"))
        {
            var body = AgentHeaderParser.RemoveHeaders(response.Response.Content!);
            
            response.Arguments.Add("StageTasks", body);
        }

        return response;
    }
}