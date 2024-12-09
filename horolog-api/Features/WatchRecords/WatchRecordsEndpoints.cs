using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace horolog_api.Features.WatchRecords;

public static class WatchRecordsEndpoints
{
    public static IEndpointRouteBuilder MapWatchRecords(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/watch-records")
            .WithTags("Watch Records")
            .WithOpenApi();

        group.MapGet("/",
            async (IWatchRecordsService service, int? modelId) => await service.GetWatchRecords(modelId));

        group.MapPost("/",
            async (IWatchRecordsService service, WatchRecord watchRecord) => await service.AddWatchRecord(watchRecord));

        group.MapPatch("/{id:int}", async (IWatchRecordsService service, int id, WatchRecord watchRecord) =>
        {
            await service.PatchWatchRecord(id, watchRecord);
            return TypedResults.Ok();
        });

        group.MapPatch("/date-borrowed/{id:int}",
            async (IWatchRecordsService service, int id) =>
            {
                await service.SetDateBorrowedToNull(id);
                return TypedResults.Ok();
            });

        group.MapDelete("/{id:int}", async (IWatchRecordsService service, int id) =>
        {
            var affectedRows = await service.DeleteWatchRecord(id);
            return affectedRows == 0 ? Results.Problem(statusCode: 404) : TypedResults.NoContent();
        });

        group.MapGet("/count", async (IWatchRecordsService service) => await service.GetWatchRecordsCount());

        return endpoints;
    }
}