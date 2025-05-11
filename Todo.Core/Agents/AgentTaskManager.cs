using Todo.Core.Communication;

namespace Todo.Core.Agents
{
    public class AgentTaskManager(IAgent agent)
    {
        public async Task<SendTaskResponse> SendTask(SendTaskRequest request)
        {
            var textPart = request.Parameters.Message.Parts.First();

            var response = await agent.InvokeAsync(new ChatCompletionRequest { Message = textPart.Text });
            
            return new SendTaskResponse { Message = response.Message };
        }
    }
}
