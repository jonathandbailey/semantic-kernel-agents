using MediatR;
using Todo.ConsoleApp.Commands;
using Todo.ConsoleApp.Settings;
using Todo.Core.Messaging;

namespace Todo.ConsoleApp;

public class TodoApplication(ICommandDispatcher commandManager) : INotificationHandler<AssistantMessage>
{
    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
        commandManager.Initialize(cancellationTokenSource);

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            Console.Write(Constants.UserCaret);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input)) continue;

            await commandManager.ExecuteCommandAsync(input);
        }
    }

    public Task Handle(AssistantMessage notification, CancellationToken cancellationToken)
    {
        Console.WriteLine(notification.Message);

        return Task.CompletedTask;
    }
}