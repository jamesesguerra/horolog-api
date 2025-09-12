using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.Users;

public class UsersRepository(IDbContext context) : IUsersRepository
{
    public async Task<User> AddUser(User user)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" INSERT INTO User (Username, PasswordHash, PasswordSalt)
                     VALUES (@Username, @PasswordHash, @PasswordSalt);";
        
        await connection.ExecuteAsync(sql, user);
        user.Id = connection.ExecuteScalar<int>("SELECT last_insert_rowid();");

        return user;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        using var connection = context.CreateConnection();

        var sql = @" SELECT Id, Username, PasswordHash, PasswordSalt
                     FROM User
                     WHERE Username = @Username ";

        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
    }
}