namespace horolog_api.Features.WatchReports;

public class WatchReportsService(IWatchReportsRepository repository) : IWatchReportsService
{
    public async Task<IEnumerable<WatchSalesReportDto>> GetBestSellingWatches()
    {
        return await repository.GetBestSellingWatches();
    }

    public async Task<long> GetTotalValue()
    {
        return await repository.GetTotalValue();
    }

    public async Task<long> GetAverageValue()
    {
        return await repository.GetAverageValue();
    }

    public async Task<IEnumerable<BrandWatchSummaryDto>> GetBrandWatchSummary()
    {
        return await repository.GetBrandWatchSummary();
    }
}