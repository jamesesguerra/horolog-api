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

    public async Task<WatchMetricsDto> GetWatchMetrics()
    {
        using var connection = context.CreateConnection();

        var sql = @"
            SELECT COUNT(*) FROM WatchRecord WHERE DateSold IS NULL AND IsConsigned = 0;
            SELECT SUM(Cost) FROM WatchRecord WHERE DateSold IS NULL AND IsConsigned = 0;
            SELECT AVG(Cost) FROM WatchRecord WHERE DateSold IS NULL;
            SELECT COUNT(Id) FROM WatchRecord WHERE DateSold IS NOT NULL;
        ";

        var multi = await connection.QueryMultipleAsync(sql);

        var totalCount = await multi.ReadSingleAsync<int>();
        var totalValue = await multi.ReadSingleAsync<long>();
        var averageValue = await multi.ReadSingleAsync<long>();
        var totalSold = await multi.ReadSingleAsync<int>();

        return new WatchMetricsDto
        {
            TotalCount = totalCount,
            TotalValue = totalValue,
            AverageValue = averageValue,
            TotalSold = totalSold
        };
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

    public async Task<IEnumerable<MonthlySalesDto>> GetMonthlySales()
    {
        using var connection = context.CreateConnection();
        var sql = @"
            SELECT 
                CASE strftime('%m', datesold)
                    WHEN '01' THEN 'Jan'
                    WHEN '02' THEN 'Feb'
                    WHEN '03' THEN 'Mar'
                    WHEN '04' THEN 'Apr'
                    WHEN '05' THEN 'May'
                    WHEN '06' THEN 'Jun'
                    WHEN '07' THEN 'Jul'
                    WHEN '08' THEN 'Aug'
                    WHEN '09' THEN 'Sep'
                    WHEN '10' THEN 'Oct'
                    WHEN '11' THEN 'Nov'
                    WHEN '12' THEN 'Dec'
                END AS MonthName,
                COUNT(*) AS TotalSold
            FROM watchrecord
            WHERE datesold IS NOT NULL
            AND strftime('%Y', datesold) = strftime('%Y', 'now')
            GROUP BY strftime('%m', datesold)
            ORDER BY strftime('%m', datesold);
        ";

        return await connection.QueryAsync<MonthlySalesDto>(sql);
    }

    public async Task<IEnumerable<BrandInventoryCountDto>> GetBrandInventoryCount()
    {
        using var connection = context.CreateConnection();
        var sql = @"
            SELECT 
                b.Name AS BrandName,
                COUNT(wr.Id) AS TotalCount
            FROM Brand b
            LEFT JOIN WatchModel wm ON wm.BrandId = b.Id
            LEFT JOIN WatchRecord wr 
                ON wr.ModelId = wm.Id 
            AND wr.DateSold IS NULL 
            AND wr.IsConsigned = 0
            GROUP BY b.Id, b.Name
            HAVING COUNT(wr.Id) > 0
            ORDER BY TotalCount DESC;
        ";

        return await connection.QueryAsync<BrandInventoryCountDto>(sql);
    }

    public async Task<InventoryBreakdownDto> GetInventoryBreakdown()
    {
        using var connection = context.CreateConnection();
        var sql = @"
            SELECT COUNT(Id) FROM WatchRecord WHERE DateSold IS NULL AND IsConsigned = 0;
            SELECT COUNT(Id) FROM WatchRecord WHERE DateSold IS NOT NULL;
            SELECT COUNT(Id) FROM WatchRecord WHERE DateSold IS NULL AND IsConsigned = 1;
        ";

        var multi = await connection.QueryMultipleAsync(sql);

        var unsoldCount = await multi.ReadSingleAsync<int>();
        var soldCount = await multi.ReadSingleAsync<int>();
        var consignedCount = await multi.ReadSingleAsync<int>();

        return new InventoryBreakdownDto
        {
            UnsoldCount = unsoldCount,
            SoldCount = soldCount,
            ConsignedCount = consignedCount
        };
    }
}