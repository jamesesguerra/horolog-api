namespace horolog_api.Features.Users;

public interface IUsersService
{
    Task<User> AddUser(User user);
    Task<User?> GetUserByUsername(string username);
}