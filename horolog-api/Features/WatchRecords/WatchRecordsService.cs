using System.Text.Json;
using Microsoft.Data.SqlClient;

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
        catch (SqlException)
        {
            logger.LogError("Add watch record called with: {0}", JsonSerializer.Serialize(watchRecord));
            throw;
        }
    }

    public async Task PatchWatchRecord(int id, WatchRecord watchRecord)
    {
        await repository.PatchWatchRecord(id, watchRecord);
    }

    public async Task SetDateBorrowedToNull(int id)
    {
        await repository.SetDateBorrowedToNull(id);
    }

    public async Task SetDateSoldToNull(int id)
    {
        await repository.SetDateSoldToNull(id);
    }

    public async Task<int> DeleteWatchRecord(int id)
    {
        return await repository.DeleteWatchRecord(id);
    }

    public async Task<int> GetWatchRecordsCount()
    {
        return await repository.GetWatchRecordsCount();
    }
}