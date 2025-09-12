using Microsoft.Data.Sqlite;

namespace horolog_api.Data;

public interface IDbContext
{
    SqliteConnection CreateConnection();
}