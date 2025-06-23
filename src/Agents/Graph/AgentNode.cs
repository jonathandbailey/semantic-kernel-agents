using Agents.Communication;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using Agents.Build;
using Microsoft.SemanticKernel;
using Todo.Infrastructure;

namespace Agents.Graph
{
    public class AgentNode(string name, IAgentProvider agentProvider, Func<StreamingChatMessageContent, string, bool, Task> streamingMessageCallback) : INode
    {
        public async Task<AgentState> InvokeAsync(AgentState state)
        {
            var agent = await agentProvider.Create(Name, async (content, isEndOfStream) =>
            {
                await streamingMessageCallback(content, state.GetSessionId(), isEndOfStream);
            });

            var responseState = await agent.InvokeAsync(state);

            if (AgentHeaderParser.HasHeader(AgentHeaderParser.AgentInvokeHeader,
                   responseState.Responses.First().Content!))
            {
                var content = AgentHeaderParser.RemoveHeaders(responseState.Responses.First().Content!);

                var agentActionTask = GetAgentTaskRequest(content);

                var workerState = CreateWorkerState(responseState, agentActionTask);

                workerState.Set("source", agentActionTask.AgentName);

                return workerState;
            }

            return responseState;
        }

        private static AgentTaskRequest GetAgentTaskRequest(string message)
        {
            var agentResponse = JsonSerializer.Deserialize<AgentTaskRequest>(message);

            if (agentResponse == null)
            {
                throw new AgentException($"Failed to deserialize agent response: {message}");
            }

            return agentResponse;
        }

        private static AgentState CreateWorkerState(AgentState sourceState, AgentTaskRequest agentActionResponse)
        {
            var sessionId = sourceState.GetSessionId();

            var state = new AgentState(agentActionResponse.AgentName) { Request = new ChatMessageContent(AuthorRole.User, agentActionResponse.Message) };

            
            state.SetTaskId(Guid.NewGuid().ToString());
            

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                state.SetSessionId(sessionId);
            }

            return state;
        }

        public string Name { get; } = name;
    }
}
