using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.Users;

public class UsersRepository(IDbContext context) : IUsersRepository
{
    public async Task<User> AddUser(User user)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" INSERT INTO [dbo].[User] (Username, PasswordHash, PasswordSalt)
                     OUTPUT INSERTED.Id
                     VALUES (@Username, @PasswordHash, @PasswordSalt) ";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, user);
        user.Id = id;

        return user;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        using var connection = context.CreateConnection();

        var sql = @" SELECT Id, Username, PasswordHash, PasswordSalt
                     FROM [dbo].[User]
                     WHERE Username = @Username ";

        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
    }
}