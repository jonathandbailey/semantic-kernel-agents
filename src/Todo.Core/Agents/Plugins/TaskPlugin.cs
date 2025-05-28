using System.ComponentModel;
using Microsoft.SemanticKernel;
using Todo.Core.Agents.A2A;
using Todo.Core.Agents.Build;
using Todo.Core.Extensions;

namespace Todo.Core.Agents.Plugins
{
    public class TaskPlugin(IAgentProvider agentProvider, IAgentStateStore agentStateStore, string name) : IAgentPlugin
    {
        [KernelFunction("send_task_request")]
        [Description("Sends a Task request to an agent")]
        public async Task<string> SendTask(
         [Description("The name of the agent who will receive the Task Request.")]   string agentName, 
          [Description("The request message for the agent.")]  string message)
        {
            var sendTaskRequest = new SendTaskRequest();
            
            sendTaskRequest.Parameters.Message.Parts.Add(new TextPart { Text = message });
            sendTaskRequest.Parameters.SessionId = agentStateStore.Get(name).SessionId;
            sendTaskRequest.Parameters.Id = agentStateStore.Get(name).TaskId;

            var agentTaskManager = agentProvider.GetTaskManager(agentName);

            var response = await agentTaskManager.SendTask(sendTaskRequest);

            var text = response.ExtractTextBasedOnResponse();

            return text;
        }
    }
}
    