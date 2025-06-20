using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using System.Text.RegularExpressions;
using Todo.Agents;
using Todo.Agents.Build;
using Todo.Agents.Communication;
using Todo.Infrastructure;

namespace Todo.Application.Services;

public class OrchestrationService(IAgentProvider agentProvider) : IOrchestrationService
{
    private static readonly Regex ActionRegex = new Regex(@"\[action:\s*(.*?)\](.*?)((?=\[action:)|$)", RegexOptions.Singleline);


    public async Task<AgentState> InvokeAsync(
        string sessionId, 
        string message, 
        Dictionary<string,string> arguments, 
        Func<StreamingChatMessageContent, string, bool, Task> streamingMessageCallback,
        string source)
    {

        var agentName = string.IsNullOrEmpty(source) ? AgentNames.UserAgent : source;
        
        var rootAgent = await agentProvider.Create(agentName, async (content, isEndOfStream) =>
        {
            await streamingMessageCallback(content, sessionId, isEndOfStream);
        });

        var userState = CreateState(agentName, sessionId, message, arguments);

        userState = await rootAgent.InvokeAsync(userState);

        var action = Parse(userState.Responses.First().Content!);

        if (action.Action == "agent_invoke")
        {
            var agentActionTask = GetAgentTaskRequest(action.Message);
            
            var workerAgent = await agentProvider.Create(agentActionTask.AgentName, async (content, isEndOfStream) =>
            {
                await streamingMessageCallback(content, sessionId, isEndOfStream);
            });

            var agentWorkerState = CreateWorkerState(userState, agentActionTask);

            var workerResponseState = await workerAgent.InvokeAsync(agentWorkerState);

            return workerResponseState;
        }


        return userState;
    }

    private static AgentTaskRequest GetAgentTaskRequest(string message)
    {
        var agentResponse = JsonSerializer.Deserialize<AgentTaskRequest>(message);

        if (agentResponse == null)
        {
            throw new AgentException($"Failed to deserialize agent response: {message}");
        }

        return agentResponse;
    }

    private static AgentState CreateState(string agentName, string sessionId, string message,
        Dictionary<string, string> arguments)
    {
        var state = new AgentState(agentName)
        {
            Request = new ChatMessageContent(AuthorRole.User, message),
            Arguments = arguments
        };

        state.SetTaskId(Guid.NewGuid().ToString());

        state.SetSessionId(!string.IsNullOrWhiteSpace(sessionId) ? sessionId : Guid.NewGuid().ToString());

        return state;
    }

    private static AgentState CreateWorkerState(AgentState orchestrationState, AgentTaskRequest agentActionResponse)
    {
        var taskId = Guid.NewGuid().ToString();
        var sessionId = orchestrationState.GetSessionId();

        var state = new AgentState(agentActionResponse.AgentName) { Request = new ChatMessageContent(AuthorRole.User, agentActionResponse.Message) };

        if (!string.IsNullOrWhiteSpace(taskId))
        {
            state.SetTaskId(taskId);
        }

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            state.SetSessionId(sessionId);
        }

        return state;
    }

    private static AgentActionResponse Parse(string input)
    {
        var match = ActionRegex.Match(input);
    
        if (!match.Success)
        {
            throw new ArgumentException("No valid action found in the input.");
        }

        var actionType = match.Groups[1].Value.Trim();
        var content = match.Groups[2].Value.Trim();

        return new AgentActionResponse() { Action = actionType, Message = content };
    }
}



public interface IOrchestrationService
{
    Task<AgentState> InvokeAsync(string sessionId, string message, Dictionary<string,string> arguments, Func<StreamingChatMessageContent, string, bool, Task> streamingMessageCallback, string source);
}