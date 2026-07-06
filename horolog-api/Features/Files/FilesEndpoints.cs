using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace horolog_api.Features.Files;

public static class FilesEndpoints
{
    private const string BucketName = "tabled-uploads";
    private const string KeyPrefix = "horolog/";

    public static IEndpointRouteBuilder MapFiles(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/files")
            .WithTags("Files");

        group.MapPost("", async ([FromServices] IMinioClient minio, IConfiguration configuration, IFormFile blob) =>
        {
            var objectName = $"{KeyPrefix}{blob.FileName}";

            await using var fileStream = blob.OpenReadStream();
            await minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(BucketName)
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

            var publicUrl = configuration["R2:PublicUrl"];
            var objectName = blobName.StartsWith(publicUrl!)
                ? blobName[publicUrl!.Length..].TrimStart('/')
                : blobName.TrimStart('/');

            await minio.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName));

            return Results.NoContent();
        });

        return endpoints;
    }
}