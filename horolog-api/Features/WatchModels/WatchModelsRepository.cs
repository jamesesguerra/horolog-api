using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchModels;

public class WatchModelsRepository(IDbContext context) : IWatchModelsRepository
{
    private static readonly List<string> IndependentBrands =
    [
        "Jaeger Lecoultre",
        "Hublot",
        "MB&F",
        "Richard Mille",
        "MCT",
        "Urwerk",
        "Blancpain",
        "Franck Muller",
        "DYW",
        "Bell & Ross",
        "Bulgari",
        "Chopard",
        "Girard-Perregaux",
        "Hermes",
        "F.P. Journe",
        "Zenith",
        "Breguet"
    ];
    
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

    public async Task<IEnumerable<int>> GetIndependentBrandModelIds()
    {
        using var connection = context.CreateConnection();
        
        var sql = @" SELECT WM.Id, B.Name
                     FROM WatchModel WM
                     JOIN Brand B ON WM.BrandId = B.Id
                     WHERE B.Name IN @IndependentBrands ";
        
        return await connection.QueryAsync<int>(sql, new { IndependentBrands });
    }
}