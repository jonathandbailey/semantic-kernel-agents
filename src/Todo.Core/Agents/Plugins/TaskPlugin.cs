using System.ComponentModel;
using Microsoft.SemanticKernel;
using Todo.Core.Agents.Build;
using Todo.Core.Communication;
using Todo.Core.Extensions;

namespace Todo.Core.Agents.Plugins
{
    public class TaskPlugin(IAgentStateStore agentStateStore, string name, IAgentPublisher publisher) : IAgentPlugin
    {
        [KernelFunction("send_task_request")]
        [Description("Sends a Task request to an agent")]
        public async Task<string> SendTask(
         [Description("The name of the agent who will receive the Task Request.")]   string agentName, 
          [Description("The request message for the agent.")]  string message)
        {
            var sendTaskRequest = AgentExtensions.CreateSendTaskRequest(agentStateStore.Get(name).TaskId,
                agentStateStore.Get(name).SessionId, message);
       
            sendTaskRequest.AgentName = agentName;
            
            var response = await publisher.Send(sendTaskRequest);

            var text = response.ExtractTextBasedOnResponse();

            return text;
        }
    }
}
    