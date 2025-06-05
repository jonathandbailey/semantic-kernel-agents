namespace Todo.Core.Users
{
    public class User : IUser
    {
        public Guid Id { get; set; }

        public string Firstname { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;
    }
}
