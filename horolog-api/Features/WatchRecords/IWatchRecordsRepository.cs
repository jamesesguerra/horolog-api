namespace horolog_api.Features.WatchRecords;

public interface IWatchRecordsRepository
{
    Task<IEnumerable<WatchRecord>> GetWatchRecords(int? modelId);
    Task<WatchRecord> AddWatchRecord(WatchRecord watchRecord);
    Task PatchWatchRecord(int id, WatchRecord watchRecord);
    Task SetDateBorrowedToNull(int id);
    Task SetDateSoldToNull(int id);
    Task<int> DeleteWatchRecord(int id);
    Task<int> GetWatchRecordsCount();
}