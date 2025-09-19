using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace horolog_api.Features.Files;

public static class FilesEndpoints
{
    public static IEndpointRouteBuilder MapFiles(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/files")
            .WithTags("Files");

        group.MapPost("", async ([FromServices] IMinioClient minio, IFormFile blob) =>
        {
            var bucketName = "horolog";
            var objectName = blob.FileName;

            await using var fileStream = blob.OpenReadStream();
            await minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(blob.Length)
                .WithContentType(blob.ContentType));

            var url = $"/{bucketName}/{objectName}";
            return Results.Ok(new { Uri = url });
        }).DisableAntiforgery();

        group.MapDelete("", async ([FromServices] IMinioClient minio, string blobName) =>
        {
            if (string.IsNullOrWhiteSpace(blobName)) return Results.NoContent();
    
            var bucketName = "horolog";

            await minio.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(blobName));

            return Results.NoContent();
        });
        
        return endpoints;
    }
}