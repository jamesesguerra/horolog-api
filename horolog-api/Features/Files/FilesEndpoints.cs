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

        group.MapPost("", async ([FromServices] IMinioClient minio, IConfiguration configuration, IFormFile blob) =>
        {
            var objectName = $"{R2Storage.KeyPrefix}{blob.FileName}";

            await using var fileStream = blob.OpenReadStream();
            await minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(R2Storage.BucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(blob.Length)
                .WithContentType(blob.ContentType));

            var publicUrl = configuration["R2:PublicUrl"];
            var url = $"{publicUrl}/{objectName}";
            return Results.Ok(new { Uri = url });
        }).DisableAntiforgery();

        group.MapDelete("", async ([FromServices] IMinioClient minio, IConfiguration configuration, string blobName) =>
        {
            if (string.IsNullOrWhiteSpace(blobName)) return Results.NoContent();

            var objectName = R2Storage.ResolveKey(blobName, configuration["R2:PublicUrl"]);

            await minio.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(R2Storage.BucketName)
                .WithObject(objectName));

            return Results.NoContent();
        });

        return endpoints;
    }
}