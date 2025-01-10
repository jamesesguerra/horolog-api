using System.Security.Cryptography.Pkcs;
using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.Brands;

public class BrandsRepository(IDbContext context) : IBrandsRepository
{
    public async Task<IEnumerable<Brand>> GetBrands()
    {
        using var connection = context.CreateConnection();

        var sql = @" SELECT Id, Name, ImageUrl, CreatedAt FROM Brand ";
        var brands = await connection.QueryAsync<Brand>(sql);

        return brands;
    }

    public async Task<Brand> GetBrandById(int id)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" SELECT Id, Name, ImageUrl, CreatedAt
                     FROM Brand
                     WHERE Id = @Id ";
        
        var brand = await connection.QuerySingleOrDefaultAsync<Brand>(sql, new { Id = id });
        return brand;
    }
}