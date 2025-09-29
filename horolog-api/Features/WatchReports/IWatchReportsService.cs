namespace horolog_api.Features.WatchReports;

public interface IWatchReportsService
{
    Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches();
    Task<WatchMetricsDto> GetWatchMetrics();
    Task<IEnumerable<BrandWatchSummaryDto>> GetBrandWatchSummary();
    Task<IEnumerable<MonthlySalesDto>> GetMonthlySales();
    Task<IEnumerable<BrandInventoryCountDto>> GetBrandInventoryCount();
    Task<InventoryBreakdownDto> GetInventoryBreakdown();
}