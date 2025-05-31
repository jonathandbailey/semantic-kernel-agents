using Microsoft.Extensions.Logging;
using System.Text.Json;
using Todo.Core.Communication;
using Todo.Core.Extensions;

namespace Todo.Core.Agents.Middleware
{
    public class Agent2AgentMiddleWare(ILogger<IAgent> logger, string agentName, IAgentPublisher publisher) : IAgentMiddleware
    {
        public async Task<AgentState> InvokeAsync(AgentState context, AgentDelegate next)
        {
            var agentTaskRequest = GetAgentResponse(context);

            var sendTaskRequest = AgentExtensions.CreateSendTaskRequest(context.TaskId,
                context.TaskId, agentTaskRequest.Message);

            sendTaskRequest.AgentName = agentTaskRequest.AgentName;

            var response = await publisher.Send(sendTaskRequest);

            context.AgentTask.SetTaskState(response.Task);

            return await next(context);
        }

        private AgentTaskRequest GetAgentResponse(AgentState state)
        {
            var agentResponse = JsonSerializer.Deserialize<AgentTaskRequest>(state.ChatCompletionResponse?.Message!);

            if (agentResponse == null)
            {
                logger.LogError($"{agentName} Failed to deserialize agent response: {state.ChatCompletionResponse?.Message!}");

                throw new AgentException($"{agentName} :Failed to deserialize agent response: {state.ChatCompletionResponse?.Message!}");
            }

            return agentResponse;
        }
    }
}
