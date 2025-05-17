using System.Net;

namespace Todo.Core;

public class AgentException : Exception
{
    public AgentException()
    {
    }

    public AgentException(string message) : base(message)
    {
    }

    public AgentException(string message, Exception inner) : base(message, inner)
    {
    }

    public AgentException(string message, Exception inner, HttpStatusCode? exceptionStatusCode) : base(message, inner)
    {
        ExceptionStatusCode = exceptionStatusCode;
    }

    public HttpStatusCode? ExceptionStatusCode { get; }
}