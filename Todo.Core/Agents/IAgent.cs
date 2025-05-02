namespace Todo.Core.Agents;

public interface IAgent
{
    Task Chat(string userInput);
}