using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Agents.A2A;
using Todo.Core.Utilities;

namespace Todo.Core.Extensions
{
    public static class AgentExtensions
    {
        public static ChatMessageContent ToChatMessageContent(this Message message)
        {
            return new ChatMessageContent
            {
                Role = new AuthorRole(message.Role),
                Content = message.Parts.First().Text
            };
        }

        public static Message ToMessage(this ChatMessageContent chatMessageContent)
        {
            Verify.NotNullOrWhiteSpace(chatMessageContent.Content);
            
            return new Message()
            {
                Role = chatMessageContent.Role.ToString(),
                Parts =
                [
                    new TextPart
                    {
                        Text = chatMessageContent.Content
                    }
                ]
            };
        }

        public static AgentTask SetInputRequiredState(this AgentTask agentTask, string text)
        {
            agentTask.Status = new AgentTaskStatus
            {
                State = AgentTaskState.InputRequired,
                Message = new Message
                {
                    Role = "agent",
                    Parts = [new TextPart { Text = text }]
                }
            };

            return agentTask;
        }

        public static AgentTask SetCompletedState(this AgentTask agentTask, string text)
        {
            agentTask.Status = new AgentTaskStatus
            {
                State = "completed"
            };
            agentTask.Artifacts.Add(new AgentArtifact
            {
                Parts = [new TextPart { Text = text }],
            });
            return agentTask;
        }

        public static AgentTask CreateAgentTask(this SendTaskRequest request)
        {
            var agentTask = new AgentTask
            {
                SessionId = request.Parameters.SessionId,
                TaskId = Guid.NewGuid().ToString(),
            };

            agentTask.History.Add(request.Parameters.Message);

            return agentTask;
        }
    }
}
