using horolog_api.Features.WatchModels;

namespace horolog_api.Features.WatchRecords;

public class WatchRecordsService(IWatchRecordsRepository repository) : IWatchRecordsService
{
    public async Task<IEnumerable<WatchRecord>> GetWatchRecords(int? modelId)
    {
        return await repository.GetWatchRecords(modelId);
    }

    public async Task<WatchRecord> AddWatchRecord(WatchRecord watchRecord)
    {
        return await repository.AddWatchRecord(watchRecord);
    }

    public async Task PatchWatchRecord(int id, WatchRecord watchRecord)
    {
        await repository.PatchWatchRecord(id, watchRecord);
    }

    public async Task SetDateBorrowedToNull(int id)
    {
        await repository.SetDateBorrowedToNull(id);
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