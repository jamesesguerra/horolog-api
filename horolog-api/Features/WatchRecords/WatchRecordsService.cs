using horolog_api.Features.WatchModels;

namespace horolog_api.Features.WatchRecords;

public class WatchRecordsService(IWatchRecordsRepository repository) : IWatchRecordsService
{
    public async Task<IEnumerable<WatchRecord>> GetWatchRecordsByModelId(int id)
    {
        return await repository.GetWatchRecordsByModelId(id);
    }

    public async Task<WatchRecord> AddWatchRecord(WatchRecord watchRecord)
    {
        return await repository.AddWatchRecord(watchRecord);
    }

    public async Task PatchWatchRecord(int id, WatchRecord watchRecord)
    {
        await repository.PatchWatchRecord(id, watchRecord);
    }
}