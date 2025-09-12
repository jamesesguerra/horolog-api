using System.Data;
using Microsoft.Data.Sqlite;

namespace horolog_api.Data;

public static class DbInitializer
{
    public static void Initialize(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dbPath = builder.DataSource;
        var folder = Path.GetDirectoryName(dbPath);
        
        if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            Console.WriteLine($"Created folder: {folder}");
        }
        
        using var connection = new SqliteConnection(connectionString);

        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "PRAGMA foreign_keys = ON;";
        cmd.ExecuteNonQuery();
        
        CreateBrandTable(connection);
        CreateWatchModelTable(connection);
        CreateWatchRecordTable(connection);
        CreateWatchImageTable(connection);
        CreateUserTable(connection);
    }
    
     private static void CreateBrandTable(IDbConnection conn)
        {
            var sql = @"
            CREATE TABLE IF NOT EXISTS Brand (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                ImageUrl TEXT NOT NULL,
                CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );";
            conn.ExecuteCommand(sql);
        }

        private static void CreateWatchModelTable(IDbConnection conn)
        {
            var sql = @"
            CREATE TABLE IF NOT EXISTS WatchModel (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BrandId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (BrandId) REFERENCES Brand(Id)
            );";
            conn.ExecuteCommand(sql);
        }

        private static void CreateWatchRecordTable(IDbConnection conn)
        {
            var sql = @"
            CREATE TABLE IF NOT EXISTS WatchRecord (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ModelId INTEGER NOT NULL,
                Description TEXT,
                Material TEXT,
                DatePurchased TEXT,
                DateReceived TEXT,
                DateSold TEXT,
                DateBorrowed TEXT,
                DateReturned TEXT,
                ReferenceNumber TEXT,
                SerialNumber TEXT,
                Location TEXT,
                HasBox INTEGER NOT NULL DEFAULT 0,
                HasPapers INTEGER NOT NULL DEFAULT 0,
                Cost INTEGER,
                Remarks TEXT,
                CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                DatePickedUp TEXT,
                FOREIGN KEY (ModelId) REFERENCES WatchModel(Id)
            );";
            conn.ExecuteCommand(sql);
        }

        private static void CreateWatchImageTable(IDbConnection conn)
        {
            var sql = @"
            CREATE TABLE IF NOT EXISTS WatchImage (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                RecordId INTEGER NOT NULL,
                Uri TEXT NOT NULL,
                FOREIGN KEY (RecordId) REFERENCES WatchRecord(Id) ON DELETE CASCADE
            );";
            conn.ExecuteCommand(sql);
        }

        private static void CreateUserTable(IDbConnection conn)
        {
            var sql = @"
            CREATE TABLE IF NOT EXISTS User (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                PasswordHash BLOB NOT NULL,
                PasswordSalt BLOB NOT NULL
            );";
            conn.ExecuteCommand(sql);
        }

        // Helper extension method
        private static void ExecuteCommand(this IDbConnection conn, string sql)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
}