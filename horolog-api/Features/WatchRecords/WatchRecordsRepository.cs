using System.Text;
using Dapper;
using horolog_api.Data;

namespace horolog_api.Features.WatchRecords;

public class WatchRecordsRepository(IDbContext context) : IWatchRecordsRepository
{
    public async Task<IEnumerable<WatchRecord>> GetWatchRecordsByModelId(int id)
    {
        using var connection = context.CreateConnection();

        var sql = @" SELECT WR.Id, WR.ModelId, WR.ImageUrl, WR.Description, WR.Material, WR.DatePurchased,
                            WR.DateReceived, WR.DateSold, WR.ReferenceNumber, WR.SerialNumber, WR.Location,
                            WR.HasBox, WR.HasPapers, WR.Cost, WR.Remarks, WR.CreatedAt
                     FROM WatchRecord WR
                     JOIN WatchModel WM ON WR.ModelId = WM.Id
                     WHERE WM.Id = @Id ";
        
        var records = await connection.QueryAsync<WatchRecord>(sql, new { Id = id });
        return records;
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
        var dynamicParams = BuildDynamicParams(watchRecord, queryBuilder);
        queryBuilder.Append(" WHERE Id = @id ");
        dynamicParams.Add("@id", id);
        
        await connection.ExecuteAsync(queryBuilder.ToString(), dynamicParams);
    }
    
    #region private methods
    private DynamicParameters BuildDynamicParams(WatchRecord recipe, StringBuilder queryBuilder)
    {
        var dynamicParams = new DynamicParameters();
        bool isFirstParam = true;

        var properties = typeof(WatchRecord).GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(recipe);
            if (value != null && property.Name != "Id" && property.Name != "ModelId")
            {
                var fieldName = property.Name;
                var paramName = $"@{fieldName}";

                queryBuilder.Append(isFirstParam ? $" {fieldName} = {paramName} " : $", {fieldName} = {paramName} ");
                dynamicParams.Add(paramName, value);
                isFirstParam = false;
            }
        }
        
        return dynamicParams;
    }
    #endregion
}