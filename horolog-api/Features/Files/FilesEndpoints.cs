using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace horolog_api.Features.Files;

public static class FilesEndpoints
{
    public static IEndpointRouteBuilder MapFiles(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/files")
            .WithTags("Files");

        group.MapPost("", async (BlobServiceClient service, IFormFile blob) =>
        {
            var container = service.GetBlobContainerClient("horolog");
            var client = container.GetBlobClient(blob.FileName);
            var contentType = blob.ContentType;
            var blobHttpHeaders = new BlobHttpHeaders() { ContentType = contentType };
            await using var fileStream = blob.OpenReadStream();
            await client.UploadAsync(fileStream, blobHttpHeaders);

            return Results.Ok(new { Uri = client.Uri.AbsoluteUri });
        }).DisableAntiforgery();

        group.MapDelete("", async (BlobServiceClient service, string blobName) =>
        {
            if (blobName.Length == 0) return Results.NoContent();
            
            var container = service.GetBlobContainerClient("horolog");
            var client = container.GetBlobClient(blobName);
            await client.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return Results.NoContent();
        });
        
        return endpoints;
    }
}