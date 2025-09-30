using horolog_api.Helpers;

namespace horolog_api.Features.WatchReports;

public static class WatchReportsEndpoints
{
    public static void MapWatchReports(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/watch-reports")
            .WithTags("Watch Reports")
            .WithOpenApi();

        group.MapGet("/best-selling",
            async (IWatchReportsService service) => await service.GetBestSellingWatches()).AddEndpointFilter(CacheHelper.AddDayCache);

        group.MapGet("/watch-metrics", async (IWatchReportsService service) => await service.GetWatchMetrics()).AddEndpointFilter(CacheHelper.AddDayCache);

        group.MapGet("/brand-watch-summary",
            async (IWatchReportsService service) => await service.GetBrandWatchSummary());

        group.MapGet("/monthly-sales",
            async (IWatchReportsService service) => await service.GetMonthlySales()).AddEndpointFilter(CacheHelper.AddDayCache);

        group.MapGet("/brand-inventory-count",
            async (IWatchReportsService service) => await service.GetBrandInventoryCount()).AddEndpointFilter(CacheHelper.AddDayCache);

        group.MapGet("/inventory-breakdown",
            async (IWatchReportsService service) => await service.GetInventoryBreakdown()).AddEndpointFilter(CacheHelper.AddDayCache);

        group.MapGet("/monthly-trend",
            async (IWatchReportsService service) => await service.GetMonthlyBrandPriceTrend()).AddEndpointFilter(CacheHelper.AddDayCache);
        
        group.MapGet("/box-papers-status",
            async (IWatchReportsService service) => await service.GetBoxPapersStatus()).AddEndpointFilter(CacheHelper.AddDayCache);
    }
}