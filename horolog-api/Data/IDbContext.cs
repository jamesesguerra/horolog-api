using System.Data;
using Microsoft.Data.SqlClient;

namespace horolog_api.Data;

public interface IDbContext
{
    SqlConnection CreateConnection();
}