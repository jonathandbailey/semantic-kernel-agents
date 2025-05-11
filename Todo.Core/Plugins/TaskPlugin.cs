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
        public async Task SendTask(string agentName, string message)
        {
            var sendTaskRequest = new SendTaskRequest();

            sendTaskRequest.Parameters.Message.Parts.Add(new TextPart { Text = message });

            var agentTaskManager = agentProvider.GetTaskManager(agentName);

            await agentTaskManager.SendTask(sendTaskRequest);
        }
    }
}
