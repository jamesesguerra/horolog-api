namespace horolog_api.Features.WatchReports;

public interface IWatchReportsService
{
    Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches();
    Task<long> GetTotalValue();
    Task<long> GetAverageValue();
}