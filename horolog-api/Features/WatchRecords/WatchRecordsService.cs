using System.Text.Json;
using horolog_api.Features.Files;
using horolog_api.Features.WatchImages;
using Microsoft.Data.Sqlite;
using Minio;
using Minio.DataModel.Args;

namespace horolog_api.Features.WatchRecords;

public class WatchRecordsService(
    IWatchRecordsRepository repository,
    IWatchImagesRepository watchImagesRepository,
    IMinioClient minio,
    IConfiguration configuration,
    ILogger<WatchRecordsService> logger) : IWatchRecordsService
{
    public async Task<IEnumerable<WatchRecord>> GetWatchRecords(int? modelId)
    {
        return await repository.GetWatchRecords(modelId);
    }

    public async Task<WatchRecord> AddWatchRecord(WatchRecord watchRecord)
    {
        try
        {
            return await repository.AddWatchRecord(watchRecord);
        }
        catch (SqliteException)
        {
            logger.LogError("Add watch record called with: {0}", JsonSerializer.Serialize(watchRecord));
            throw;
        }
    }

    public async Task PatchWatchRecord(int id, WatchRecord watchRecord)
    {
        await repository.PatchWatchRecord(id, watchRecord);
    }

    public async Task<int> DeleteWatchRecord(int id)
    {
        var images = await watchImagesRepository.GetWatchImagesByRecordId(id);
        var publicUrl = configuration["R2:PublicUrl"];

        foreach (var image in images)
        {
            if (string.IsNullOrWhiteSpace(image.Uri)) continue;

            try
            {
                var objectName = R2Storage.ResolveKey(image.Uri, publicUrl);
                await minio.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(R2Storage.BucketName)
                    .WithObject(objectName));
            }
            catch (Exception ex)
            {
                // Best-effort: don't block record deletion on a storage hiccup,
                // but keep the DB delete going so the user isn't stuck.
                logger.LogError(ex, "Failed to delete R2 object for watch image {ImageId} ({Uri})", image.Id, image.Uri);
            }
        }

        return await repository.DeleteWatchRecord(id);
    }

    public async Task SetFieldToNull(string fieldName, int id)
    {
        await repository.SetFieldToNull(fieldName, id);
    }
}