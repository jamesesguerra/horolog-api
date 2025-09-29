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

        group.MapGet("/total-value", async (IWatchReportsService service) => await service.GetTotalValue());

        group.MapGet("/average-value", async (IWatchReportsService service) => await service.GetAverageValue()).AddEndpointFilter(CacheHelper.AddDayCache);;

        group.MapGet("/brand-watch-summary",
            async (IWatchReportsService service) => await service.GetBrandWatchSummary());
            
        group.MapGet("/monthly-sales",
            async (IWatchReportsService service) => await service.GetMonthlySales()).AddEndpointFilter(CacheHelper.AddDayCache);
    }
}