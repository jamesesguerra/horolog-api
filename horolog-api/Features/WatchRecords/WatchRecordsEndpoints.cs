using Microsoft.AspNetCore.Http.HttpResults;

namespace horolog_api.Features.WatchRecords;

public static class WatchRecordsEndpoints
{
    public static IEndpointRouteBuilder MapWatchRecords(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/watch-records")
            .WithTags("Watch Records")
            .WithOpenApi();

        group.MapGet("/",
            async (IWatchRecordsService service, int modelId) => await service.GetWatchRecordsByModelId(modelId));

        group.MapPost("/",
            async (IWatchRecordsService service, WatchRecord watchRecord) => await service.AddWatchRecord(watchRecord));

        group.MapPatch("/{id:int}", async (IWatchRecordsService service, int id, WatchRecord watchRecord) =>
        {
            await service.PatchWatchRecord(id, watchRecord);
            return TypedResults.Ok();
        });
        
        return endpoints;
    }
}