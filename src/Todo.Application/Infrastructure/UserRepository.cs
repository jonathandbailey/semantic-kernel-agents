using Todo.Core.Users;

namespace Todo.Application.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        public Task<IUser> Get(Guid id)
        {
            var user = new User
            {
                Id = id,
                Firstname = "Jonathan",
                Lastname = "Bailey",
            };

            return Task.FromResult<IUser>(user);
        }
    }
    public interface IUserRepository
    {
        Task<IUser> Get(Guid id);
    }
}
