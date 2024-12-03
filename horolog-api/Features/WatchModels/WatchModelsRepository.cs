using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchModels;

public class WatchModelsRepository(IDbContext context) : IWatchModelsRepository
{
    public async Task<IEnumerable<WatchModel>> GetWatchModelsByBrandId(int id)
    {
        using var connection = context.CreateConnection();

        var sql = @" SELECT Id, BrandId, Name, CreatedAt
                     FROM WatchModel
                     WHERE BrandId = @Id ";
        
        var models = await connection.QueryAsync<WatchModel>(sql, new { Id = id });

        return models;
    }

    public async Task<WatchModel> AddWatchModel(WatchModel watchModel)
    {
        using var connection = context.CreateConnection();

        var sql = @" INSERT INTO WatchModel (BrandId, Name, CreatedAt)
                     OUTPUT INSERTED.Id
                     VALUES (@BrandId, @Name, @CreatedAt) ";
        
        var now = DateTime.Now;
        
        var id = await connection.ExecuteScalarAsync<int>(sql, new 
            { watchModel.BrandId, watchModel.Name, CreatedAt = now });

        watchModel.Id = id;
        watchModel.CreatedAt = now;

        return watchModel;
    }
}