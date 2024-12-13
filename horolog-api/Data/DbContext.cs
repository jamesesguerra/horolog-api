using System.Data;
using Microsoft.Data.SqlClient;


namespace horolog_api.Data;

public class DbContext(IConfiguration configuration) : IDbContext
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
}