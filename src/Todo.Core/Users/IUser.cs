namespace Todo.Application.Users;

public interface IUser
{
    Guid Id { get; set; }
    string Firstname { get; set; }
    string Lastname { get; set; }
}