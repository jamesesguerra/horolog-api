using Microsoft.Extensions.Caching.Memory;

namespace horolog_api.Features.WatchModels;

public static class WatchModelsEndpoints
{
    public static void MapWatchModels(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/watch-models")
            .WithTags("Watch Models")
            .WithOpenApi();

        group.MapGet("/", async (IWatchModelsService service, int brandId, IMemoryCache memoryCache) =>
        {
            var cacheKey = $"models-{brandId}";

            if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<WatchModel>? result))
            {
                result = await service.GetWatchModelsByBrandId(brandId);
                memoryCache.Set(cacheKey, result, new TimeSpan(1, 0, 0, 0));
            }

            return Results.Ok(result);
        });

        group.MapPost("/", async (IWatchModelsService service, WatchModel watchModel) => await service.AddWatchModel(watchModel));

        group.MapGet("/independent-brands", async (IWatchModelsService service) => await service.GetIndependentBrandModelIds());
    }
}