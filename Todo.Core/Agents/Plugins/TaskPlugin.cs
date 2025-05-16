using System.ComponentModel;
using Microsoft.SemanticKernel;
using Todo.Core.Agents.A2A;
using Todo.Core.Extensions;

namespace Todo.Core.Agents.Plugins
{
    public class TaskPlugin(IAgentProvider agentProvider) 
    {
        [KernelFunction("send_task_request")]
        [Description("Sends a Task request to an agent")]
        public async Task<string> SendTask(
         [Description("The name of the agent who will receive the Task Request.")]   string agentName, 
          [Description("The request mmessage for the agent.")]  string message, Kernel kernel)
        {
            var sendTaskRequest = new SendTaskRequest();

            sendTaskRequest.Parameters.Message.Parts.Add(new TextPart { Text = message });
            sendTaskRequest.Parameters.SessionId = kernel.Data["sessionId"]?.ToString() ?? Guid.NewGuid().ToString();

            var agentTaskManager = agentProvider.GetTaskManager(agentName);

            var response = await agentTaskManager.SendTask(sendTaskRequest);

            return response.ExtractText();
        }
    }
}
    