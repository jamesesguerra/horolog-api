namespace horolog_api.Features.WatchRecords;

public interface IWatchRecordsRepository
{
    Task<IEnumerable<WatchRecord>> GetWatchRecordsByModelId(int id);
    Task<WatchRecord> AddWatchRecord(WatchRecord watchRecord);
    Task PatchWatchRecord(int id, WatchRecord watchRecord);
}