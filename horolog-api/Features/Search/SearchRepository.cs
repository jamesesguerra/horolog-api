using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.Search;

public class SearchRepository(IDbContext context) : ISearchRepository
{
    public async Task<T?> SearchAsync<T>(string selectBy, string searchBy, string searchQuery)
    {
        string[] allowedSelectColumns = [
            "DateReceived",
            "DateBorrowed",
            "DateReturned"
        ]; 

        string[] allowedSearchColumns = [
            "SerialNumber",
            "ReferenceNumber",
            "Description"
        ]; 

        if (!allowedSelectColumns.Contains(selectBy))
            throw new ArgumentException("Invalid select column");

        if (!allowedSearchColumns.Contains(searchBy))
            throw new ArgumentException("Invalid search column");

        var sql = $@"
            SELECT {selectBy}
            FROM WatchRecord
            WHERE {searchBy} LIKE @SearchQuery
        ";

        using var connection = context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<T>(sql, new
        {
            SearchQuery = $"%{searchQuery}%"
        });
    }
}