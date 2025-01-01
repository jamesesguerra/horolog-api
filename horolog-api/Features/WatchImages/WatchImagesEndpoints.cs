namespace horolog_api.Features.WatchImages;

public static class WatchImagesEndpoints
{
    public static IEndpointRouteBuilder MapWatchImages(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/watch-images")
            .WithTags("Watch Images")
            .WithOpenApi();

        group.MapPost("/",
            async (IWatchImagesService service, HttpContext context) =>
            {
                var watchImages = await context.Request.ReadFromJsonAsync<List<WatchImage>>();

                if (watchImages is null || !watchImages.Any()) return TypedResults.BadRequest();
                
                var affectedRows = await service.AddWatchImages(watchImages);
                return affectedRows == 0 ? Results.BadRequest() : Results.Created();
            });

        group.MapGet("/", async (IWatchImagesService service, int recordId) => await service.GetWatchImagesByRecordId(recordId));

        group.MapDelete("/{id}", async (IWatchImagesService service, int id) =>
        {
            var affectedRows = await service.DeleteWatchImage(id);
            return affectedRows == 0 ? Results.Problem(statusCode: 404) : TypedResults.NoContent();
        });
        
        return endpoints;
    }
}