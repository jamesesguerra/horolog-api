using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchImages;

public class WatchImagesRepository(IDbContext context) : IWatchImagesRepository
{
    public async Task<int> AddWatchImages(List<WatchImage> watchImages)
    {
        var sql = @" INSERT INTO WatchImage (RecordId, Uri)
                     VALUES (@RecordId, @Uri) ";
        
        await using var connection = context.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            int rowsAffected = await connection.ExecuteAsync(sql, watchImages, transaction);
            await transaction.CommitAsync();
            return rowsAffected;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<WatchImage>> GetWatchImagesByRecordId(int id)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" SELECT Id, Uri
                     FROM WatchImage
                     WHERE RecordId = @Id
                     ORDER BY Id ";

        return await connection.QueryAsync<WatchImage>(sql, new { Id = id });
    }

    public async Task<int> DeleteWatchImage(int id)
    {
        using var connection = context.CreateConnection();
        var sql = " DELETE FROM WatchImage WHERE ID = @Id ";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows;
    }
}