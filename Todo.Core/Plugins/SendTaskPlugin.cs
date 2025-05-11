using Microsoft.SemanticKernel;
using Todo.Core.Agents;
using Todo.Core.Communication;

namespace Todo.Core.Plugins
{
    public class SendTaskPlugin(IAgentServer agentServer)
    {
        [KernelFunction("send_task")]
        public async Task SendTask(string agentName, string message)
        {
            var sendTaskRequest = new SendTaskRequest();

            sendTaskRequest.Parameters.Message.Parts.Add(new TextPart { Text = message });

            await agentServer.SendTask(agentName, sendTaskRequest);
        }
    }
}
