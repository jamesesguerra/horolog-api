using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace horolog_api.Features.WatchRecords;

public class WatchRecordsService(
    IWatchRecordsRepository repository,
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
        return await repository.DeleteWatchRecord(id);
    }

    public async Task<int> GetWatchRecordsCount()
    {
        return await repository.GetWatchRecordsCount();
    }

    public async Task SetFieldToNull(string fieldName, int id)
    {
        await repository.SetFieldToNull(fieldName, id);
    }
}