using Agents.Build;
using Microsoft.SemanticKernel;

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

            responseState.Set("source", Name);

            return responseState;
        }

        public string Name { get; } = name;
    }
}
