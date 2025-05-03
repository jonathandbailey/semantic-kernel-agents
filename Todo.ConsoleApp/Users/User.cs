using Todo.Core.Users;

namespace Todo.ConsoleApp.Users
{
    public class User : IUser
    {
        public Task Reply(string response)
        {
            Console.WriteLine(response);

            return Task.CompletedTask;
        }
    }
}
