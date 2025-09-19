using System.Text;
using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchRecords;

public class WatchRecordsRepository(IDbContext context) : IWatchRecordsRepository
{
    public async Task<IEnumerable<WatchRecord>> GetWatchRecords(int? modelId)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" SELECT WR.Id, WI.Uri AS ImageUrl, WR.ModelId, WR.Description, WR.Material, WR.DatePurchased,
                            WR.DateReceived, WR.DateSold, WR.DateBorrowed, WR.DateReturned, WR.DatePickedUp,
                            WR.ReferenceNumber, WR.SerialNumber, WR.Location, WR.HasBox, WR.HasPapers, WR.Cost,
                            WR.Remarks, WR.CreatedAt
                     FROM WatchRecord AS WR
                     JOIN WatchModel WM ON WR.ModelId = WM.Id
                     LEFT JOIN WatchImage WI ON WI.Id = (
                        SELECT wInner.Id
                        FROM WatchImage wInner
                        WHERE wInner.RecordId = WR.Id
                        ORDER BY wInner.Id ASC
                        LIMIT 1
                     ) ";

        var queryBuilder = new StringBuilder(sql);
        if (modelId.HasValue) queryBuilder.Append(" WHERE WM.Id = @ModelId ");
        queryBuilder.Append(" ORDER BY WR.DateSold ASC, WR.Description ASC ");

        return await connection.QueryAsync<WatchRecord>(queryBuilder.ToString(), new { ModelId = modelId });
    }

    public async Task<WatchRecord> AddWatchRecord(WatchRecord watchRecord)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" INSERT INTO WatchRecord (
                        ModelId, Description, Material, DatePurchased,
                        DateReceived, DateSold, ReferenceNumber, SerialNumber, 
                        Location, HasBox, HasPapers, Cost, Remarks, CreatedAt
                     )
                     VALUES (
                        @ModelId, @Description, @Material, @DatePurchased,
                        @DateReceived, @DateSold, @ReferenceNumber, @SerialNumber,
                        @Location, @HasBox, @HasPapers, @Cost, @Remarks, @CreatedAt
                     )";

        var now = DateTime.Now;
        await connection.ExecuteScalarAsync<int>(sql, new
        {
            watchRecord.ModelId,
            watchRecord.Description,
            watchRecord.Material,
            watchRecord.DatePurchased,
            watchRecord.DateReceived,
            watchRecord.DateSold,
            watchRecord.ReferenceNumber,
            watchRecord.SerialNumber,
            watchRecord.Location,
            watchRecord.HasBox,
            watchRecord.HasPapers,
            watchRecord.Cost,
            watchRecord.Remarks,
            CreatedAt = now
        });

        var id = await connection.ExecuteScalarAsync<int>("SELECT last_insert_rowid();");
        watchRecord.Id = id;
        watchRecord.CreatedAt = now;
        return watchRecord;
    }

    public async Task PatchWatchRecord(int id, WatchRecord watchRecord)
    {
        using var connection = context.CreateConnection();
        
        var queryBuilder = new StringBuilder(" UPDATE WatchRecord SET ");
        var dynamicParams = BuildDynamicPatchParams(watchRecord, queryBuilder);
        queryBuilder.Append(" WHERE Id = @id ");
        dynamicParams.Add("@id", id);
        
        await connection.ExecuteAsync(queryBuilder.ToString(), dynamicParams);
    }

    public async Task SetDateBorrowedToNull(int id)
    {
        using var connection = context.CreateConnection();
        var sql = " UPDATE WatchRecord SET DateBorrowed = NULL WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task SetDateSoldToNull(int id)
    {
        using var connection = context.CreateConnection();
        var sql = " UPDATE WatchRecord SET DateSold = NULL WHERE Id = @Id ";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<int> DeleteWatchRecord(int id)
    {
        using var connection = context.CreateConnection();
        var sql = " DELETE FROM WatchRecord WHERE Id = @Id ";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows;
    }

    public async Task<int> GetWatchRecordsCount()
    {
        using var connection = context.CreateConnection();
        
        var sql = " SELECT COUNT(*) FROM WatchRecord WHERE DateSold IS NULL AND DateBorrowed IS NULL ";
        
        var count = await connection.ExecuteScalarAsync<int>(sql);
        return count;
    }
    
    #region private methods
    private DynamicParameters BuildDynamicPatchParams(WatchRecord resource, StringBuilder queryBuilder)
    {
        var dynamicParams = new DynamicParameters();
        bool isFirstParam = true;

        var properties = typeof(WatchRecord).GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(resource);
            if (value == null || property.Name == "Id") continue;
            var fieldName = property.Name;
            var paramName = $"@{fieldName}";

            queryBuilder.Append(isFirstParam ? $" {fieldName} = {paramName} " : $", {fieldName} = {paramName} ");
            dynamicParams.Add(paramName, value);
            isFirstParam = false;
        }
        
        return dynamicParams;
    }
    #endregion
}