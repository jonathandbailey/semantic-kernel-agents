using MediatR;
using Todo.Core.Agents;
using Todo.Core.Agents.A2A;
using Todo.Core.Agents.Build;
using Todo.Core.Users;

namespace Todo.Core.Services;

public class TodoService(IAgentProvider agentProvider) : ITodoService, IRequestHandler<UserRequest, UserResponse>
{   
    public async Task<UserResponse> Handle(UserRequest notification, CancellationToken cancellationToken)
    {
        await agentProvider.Build();

        var sendTaskRequest = new SendTaskRequest
        {
            Parameters = new TaskSendParameters
            {
                SessionId = notification.SessionId,
                Message = new Message
                {
                    Parts = [new TextPart { Text = notification.Message }],
                    Role = "user"
                }
            }
        };

        var agentTaskManager = agentProvider.GetTaskManager(AgentNames.OrchestratorAgent);

        var response = await agentTaskManager.SendTask(sendTaskRequest);

        if (response.Task.Status.State == AgentTaskState.Completed)
        {
            return new UserResponse
                { Message = response.Task.Artifacts.First().Parts.First().Text, SessionId = response.Task.SessionId };
        }

        if (response.Task.Status.State == AgentTaskState.InputRequired)
        {
            return new UserResponse { Message = response.Task.Status.Message.Parts.First().Text, SessionId = response.Task.SessionId };
        }

        if (response.Task.Status.State == AgentTaskState.Failed)
        {
            return new UserResponse
                { Message = response.Task.Artifacts.First().Parts.First().Text, SessionId = response.Task.SessionId };
        }

        throw new InvalidOperationException($"Invalid task state : {response.Task.Status.State}");
    }
}

public interface ITodoService
{
} 