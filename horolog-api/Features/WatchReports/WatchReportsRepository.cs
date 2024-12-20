using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchReports;

public class WatchReportsRepository(IDbContext context) : IWatchReportsRepository
{
    public async Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches()
    {
        using var connection = context.CreateConnection();

        var sql = @" SELECT TOP 5 B.Name AS BrandName, WM.Name AS ModelName, COUNT(WR.DateSold) AS SoldCount
                     FROM WatchRecord AS WR
                     JOIN WatchModel AS WM ON WR.ModelId = WM.Id
                     JOIN Brand AS B ON B.Id = WM.BrandId
                     GROUP BY WM.Name, B.Name
                     ORDER BY COUNT(DateSold) DESC ";

        var watches = await connection.QueryAsync<WatchSalesReportDto>(sql);
        return watches;
    }

    public async Task<long> GetTotalValue()
    {
        using var connection = context.CreateConnection();
        var sql = " SELECT SUM(Cost) FROM WatchRecord WHERE DateSold IS NULL ";
        
        return await connection.ExecuteScalarAsync<long>(sql);
    }

    public async Task<long> GetAverageValue()
    {
        using var connection = context.CreateConnection();
        var sql = " SELECT AVG(Cost) FROM WatchRecord WHERE DateSold IS NULL ";
        
        return await connection.ExecuteScalarAsync<long>(sql);
    }
}