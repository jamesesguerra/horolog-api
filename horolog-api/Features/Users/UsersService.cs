namespace horolog_api.Features.Users;

public class UsersService(IUsersRepository repository) : IUsersService
{
    public async Task<User> AddUser(User user)
    {
        return await repository.AddUser(user);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await repository.GetUserByUsername(username);
    }
}