using System.Text;
using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchRecords;

public class WatchRecordsRepository(IDbContext context) : IWatchRecordsRepository
{
    public async Task<IEnumerable<WatchRecord>> GetWatchRecords(int? modelId)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" SELECT WR.Id, WR.ModelId, WR.ImageUrl, WR.Description, WR.Material, WR.DatePurchased,
                            WR.DateReceived, WR.DateSold, WR.DateBorrowed, WR.DateReturned, WR.ReferenceNumber,
                            WR.SerialNumber, WR.Location, WR.HasBox, WR.HasPapers, WR.Cost, WR.Remarks, WR.CreatedAt
                     FROM WatchRecord AS WR
                     JOIN WatchModel WM ON WR.ModelId = WM.Id ";

        var queryBuilder = new StringBuilder(sql);
        if (modelId.HasValue) queryBuilder.Append(" WHERE WM.Id = @ModelId ");
        queryBuilder.Append(" ORDER BY WR.CreatedAt DESC ");

        return await connection.QueryAsync<WatchRecord>(queryBuilder.ToString(), new { ModelId = modelId });
    }

    public async Task<WatchRecord> AddWatchRecord(WatchRecord watchRecord)
    {
        using var connection = context.CreateConnection();
        
        var sql = @" INSERT INTO WatchRecord (
                        ModelId, ImageUrl, Description, Material, DatePurchased,
                        DateReceived, DateSold, ReferenceNumber, SerialNumber, 
                        Location, HasBox, HasPapers, Cost, Remarks, CreatedAt
                     )
                     OUTPUT INSERTED.Id
                     VALUES (
                        @ModelId, @ImageUrl, @Description, @Material, @DatePurchased,
                        @DateReceived, @DateSold, @ReferenceNumber, @SerialNumber,
                        @Location, @HasBox, @HasPapers, @Cost, @Remarks, @CreatedAt
                     ) ";

        var now = DateTime.Now;
        var id = await connection.ExecuteScalarAsync<int>(sql, new
        {
            watchRecord.ModelId,
            watchRecord.ImageUrl,
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
        var sql = " UPDATE WatchRecord SET DateBorrowed = NULL WHERE Id = @id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<int> DeleteWatchRecord(int id)
    {
        using var connection = context.CreateConnection();
        var sql = " DELETE FROM WatchRecord WHERE ID = @Id ";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows;
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
            if (value == null || property.Name == "Id" || property.Name == "ModelId") continue;
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