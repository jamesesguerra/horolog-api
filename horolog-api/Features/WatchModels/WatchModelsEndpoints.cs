namespace horolog_api.Features.WatchModels;

public static class WatchModelsEndpoints
{
    public static IEndpointRouteBuilder MapWatchModels(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/watch-models")
            .WithTags("Watch Models")
            .WithOpenApi();

        group.MapGet("/", (IWatchModelsService service, int brandId) => service.GetWatchModelsByBrandId(brandId));

        group.MapPost("/", (IWatchModelsService service, WatchModel watchModel) => service.AddWatchModel(watchModel));

        group.MapGet("/independent-brands", (IWatchModelsService service) => service.GetIndependentBrandModelIds());

        return endpoints;
    }
}