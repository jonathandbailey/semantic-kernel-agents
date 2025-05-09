namespace Todo.Core.Agents;

public interface IAgent
{
    Task<IEnumerable<string>> InvokeAsync(string userInput);
}