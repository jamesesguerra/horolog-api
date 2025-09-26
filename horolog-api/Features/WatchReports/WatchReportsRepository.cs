using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchReports;

public class WatchReportsRepository(IDbContext context) : IWatchReportsRepository
{
    public async Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches()
    {
        using var connection = context.CreateConnection();

        var sql = @" SELECT B.Name AS BrandName, WM.Name AS ModelName, COUNT(WR.DateSold) AS SoldCount
                     FROM WatchRecord AS WR
                     JOIN WatchModel AS WM ON WR.ModelId = WM.Id
                     JOIN Brand AS B ON B.Id = WM.BrandId
                     GROUP BY WM.Name, B.Name
                     ORDER BY COUNT(DateSold) DESC
                     LIMIT 5";

        var watches = await connection.QueryAsync<WatchSalesReportDto>(sql);
        return watches;
    }

    public async Task<long> GetTotalValue()
    {
        using var connection = context.CreateConnection();
        var sql = " SELECT SUM(Cost) FROM WatchRecord WHERE DateSold IS NULL AND IsConsigned = 0";
        
        return await connection.ExecuteScalarAsync<long>(sql);
    }

    public async Task<long> GetAverageValue()
    {
        using var connection = context.CreateConnection();
        var sql = " SELECT AVG(Cost) FROM WatchRecord WHERE DateSold IS NULL ";
        
        return await connection.ExecuteScalarAsync<long>(sql);
    }

    public async Task<IEnumerable<BrandWatchSummaryDto>> GetBrandWatchSummary()
    {
        await using var connection = context.CreateConnection();
        var sql = @"
               WITH BrandSummary AS (
            SELECT 
                B.Name AS Brand,
                COUNT(WR.Id) AS Quantity,
                SUM(CASE WHEN WR.IsConsigned = 1 THEN 0 ELSE WR.Cost END) AS TotalCost,
                SUM(CASE WHEN WR.IsConsigned = 1 THEN 1 ELSE 0 END) AS ConsignedCount
            FROM Brand B
            INNER JOIN WatchModel WM ON B.Id = WM.BrandId
            INNER JOIN WatchRecord WR ON WR.ModelId = WM.Id
            WHERE WR.DateSold IS NULL
            GROUP BY B.Name
        )
        SELECT 
            Brand,
            Quantity,
            TotalCost
        FROM BrandSummary
        WHERE NOT (Quantity = 1 AND ConsignedCount = 1)

        UNION ALL

        SELECT 
            'ALL BRANDS' AS Brand,
            COUNT(WR.Id) AS Quantity,
            SUM(CASE WHEN WR.IsConsigned = 1 THEN 0 ELSE WR.Cost END) AS TotalCost
        FROM Brand B
        INNER JOIN WatchModel WM ON B.Id = WM.BrandId
        INNER JOIN WatchRecord WR ON WR.ModelId = WM.Id
        WHERE WR.DateSold IS NULL

        ORDER BY Brand; ";

        return await connection.QueryAsync<BrandWatchSummaryDto>(sql);
    }
}