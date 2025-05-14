using System.ComponentModel;
using Microsoft.SemanticKernel;
using Todo.Core.Agents;
using Todo.Core.Communication;

namespace Todo.Core.Plugins
{
    public class TaskPlugin(IAgentProvider agentProvider) 
    {
        [KernelFunction("send_task")]
        [Description("Sends instructions to an agent")]
        public async Task<string> SendTask(
         [Description("The name of the agent who will receive the message.")]   string agentName, 
          [Description("The instructions for the agent.")]  string message, Kernel kernel)
        {
            var sendTaskRequest = new SendTaskRequest();

            sendTaskRequest.Parameters.Message.Parts.Add(new TextPart { Text = message });
            sendTaskRequest.Parameters.SessionId = kernel.Data["sessionId"]?.ToString() ?? Guid.NewGuid().ToString();

            var agentTaskManager = agentProvider.GetTaskManager(agentName);

            var response = await agentTaskManager.SendTask(sendTaskRequest);

            return response.Message;
        }
    }
}
    