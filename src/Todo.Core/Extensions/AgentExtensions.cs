using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Todo.Core.Agents.A2A;
using Todo.Core.Communication;
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

        public static SendTaskRequest CreateSendTaskRequest(string id, string sessionId, string messageText)
        {
            var sendTaskRequest = new SendTaskRequest
            {
                Parameters = new TaskSendParameters
                {
                    Id = id,
                    SessionId = sessionId,
                    Message = new Message
                    {
                        Parts = [new TextPart { Text = messageText }],
                        Role = "user"
                    }
                }
            };

            return sendTaskRequest;
        }

        public static void SetTaskState(this AgentTask agentTask, AgentActionResponse actionResponse)
        {
            if (actionResponse.Action == AgentTaskState.InputRequired)
            {
                agentTask.SetInputRequiredState(actionResponse.Message);
            }

            if (actionResponse.Action == AgentTaskState.Completed)
            {
                agentTask.SetCompletedState(actionResponse.Message);
            }

            if (actionResponse.Action == AgentTaskState.Failed)
            {
                agentTask.SetInputRequiredFailed(actionResponse.Message);
            }
        }

        public static void SetTaskState(this AgentTask agentTask, AgentTask remoteTask)
        {
            if (remoteTask.Status.State == AgentTaskState.InputRequired)
            {
                agentTask.SetInputRequiredState(remoteTask.ExtractTextBasedOnResponse());
            }

            if (remoteTask.Status.State == AgentTaskState.Completed)
            {
                agentTask.SetCompletedState(remoteTask.ExtractTextBasedOnResponse());
            }

            if (remoteTask.Status.State == AgentTaskState.Failed)
            {
                agentTask.SetInputRequiredFailed(remoteTask.ExtractTextBasedOnResponse());
            }
        }

        private static void SetInputRequiredState(this AgentTask agentTask, string text)
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
        }

        public static void SetInputRequiredFailed(this AgentTask agentTask, string text)
        {
            agentTask.Status = new AgentTaskStatus
            {
                State = AgentTaskState.Failed
            };

            agentTask.Artifacts.Add(new AgentArtifact
            {
                Parts = [new TextPart { Text = text }],
            });
        }

        private static void SetCompletedState(this AgentTask agentTask, string text)
        {
            agentTask.Status = new AgentTaskStatus
            {
                State = AgentTaskState.Completed
            };
            agentTask.Artifacts.Add(new AgentArtifact
            {
                Parts = [new TextPart { Text = text }],
            });
        }

        public static string ExtractTextBasedOnResponse(this SendTaskResponse response)
        {
            return response.Task.ExtractTextBasedOnResponse();
        }

        public static string ExtractTextBasedOnResponse(this AgentTask task)
        {
            if (task.Status.State == AgentTaskState.InputRequired)
            {
                return task.Status.Message.Parts.First().Text;
            }

            if (task.Status.State == AgentTaskState.Completed)
            {
                return task.Artifacts.First().Parts.First().Text;
            }

            if (task.Status.State == AgentTaskState.Failed)
            {
                return task.Artifacts.First().Parts.First().Text;
            }

            throw new InvalidOperationException("The task state is not valid for extraction.");
        }

        public static AgentTask CreateAgentTask(this SendTaskRequest request)
        {
            var agentTask = new AgentTask
            {
                SessionId = Guid.NewGuid().ToString(),
                TaskId = Guid.NewGuid().ToString(),
            };

            agentTask.History.Add(request.Parameters.Message);

            return agentTask;
        }


    }
}
