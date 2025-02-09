using horolog_api.Helpers;

namespace horolog_api.Features.WatchReports;

public static class WatchReportsEndpoints
{
    public static void MapWatchReports(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/watch-reports")
            .WithTags("Watch Reports")
            .WithOpenApi()
            .AddEndpointFilter(CacheHelper.AddDayCache);

        group.MapGet("/best-selling",
            async (IWatchReportsService service) => await service.GetBestSellingWatches());
        
        group.MapGet("/total-value", async (IWatchReportsService service) => await service.GetTotalValue());

        group.MapGet("/average-value", async (IWatchReportsService service) => await service.GetAverageValue());
    }
}