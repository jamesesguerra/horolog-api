using System.Data;

namespace horolog_api.Data;

public interface IDbContext
{
    IDbConnection CreateConnection();
}