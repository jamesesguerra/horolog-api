using Microsoft.Data.Sqlite;

namespace horolog_api.Data;

public class DbContext(IConfiguration configuration) : IDbContext
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public SqliteConnection CreateConnection() => new SqliteConnection(_connectionString);
}