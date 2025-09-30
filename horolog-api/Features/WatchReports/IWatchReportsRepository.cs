namespace horolog_api.Features.WatchReports;

public interface IWatchReportsRepository
{
    Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches();
    Task<WatchMetricsDto> GetWatchMetrics();
    Task<IEnumerable<BrandWatchSummaryDto>> GetBrandWatchSummary();
    Task<IEnumerable<MonthlySalesDto>> GetMonthlySales();
    Task<IEnumerable<BrandInventoryCountDto>> GetBrandInventoryCount();
    Task<InventoryBreakdownDto> GetInventoryBreakdown();
    Task<IEnumerable<BrandPriceTrendDto>> GetMonthlyBrandPriceTrend();
    Task<IEnumerable<BoxPapersDto>> GetBoxPapersStatus();
}