namespace horolog_api.Features.Users;

public interface IUsersRepository
{
    Task<User> AddUser(User user);
    Task<User?> GetUserByUsername(string username);
}