namespace horolog_api.Features.WatchReports;

public class WatchReportsService(IWatchReportsRepository repository) : IWatchReportsService
{
    public async Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches()
    {
        return await repository.GetBestSellingWatches();
    }

    public async Task<WatchMetricsDto> GetWatchMetrics()
    {
        return await repository.GetWatchMetrics();
    }

    public async Task<IEnumerable<BrandWatchSummaryDto>> GetBrandWatchSummary()
    {
        return await repository.GetBrandWatchSummary();
    }

    public async Task<IEnumerable<MonthlySalesDto>> GetMonthlySales()
    {
        return await repository.GetMonthlySales();
    }

    public async Task<IEnumerable<BrandInventoryCountDto>> GetBrandInventoryCount()
    {
        return await repository.GetBrandInventoryCount();
    }

    public async Task<InventoryBreakdownDto> GetInventoryBreakdown()
    {
        return await repository.GetInventoryBreakdown();
    }

    public async Task<IEnumerable<BrandPriceTrendDto>> GetMonthlyBrandPriceTrend()
    {
        return await repository.GetMonthlyBrandPriceTrend();
    }
}