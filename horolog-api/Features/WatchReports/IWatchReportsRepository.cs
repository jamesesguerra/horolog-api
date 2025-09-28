namespace horolog_api.Features.WatchReports;

public interface IWatchReportsRepository
{
    Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches();
    Task<long> GetTotalValue();
    Task<long> GetAverageValue();
    Task<IEnumerable<BrandWatchSummaryDto>> GetBrandWatchSummary();
    Task<IEnumerable<MonthlySalesDto>> GetMonthlySales();
}