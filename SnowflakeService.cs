using Snowflake.Data.Client;
using System.Data;

public class SnowflakeService
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly string? _privateKey;

    public SnowflakeService(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("Snowflake") ?? throw new Exception("Snowflake connection string not found.");
        _privateKey = _config["Snowflake:PrivateKey"];

        if (string.IsNullOrEmpty(_privateKey))
        {
            throw new Exception("Private key not found in environment variables.");
        }
    }

    // 🔹 Get Snowflake connection (Async)
    private async Task<SnowflakeDbConnection> GetConnectionAsync()
    {
        var conn = new SnowflakeDbConnection
        {
            ConnectionString = $"{_connectionString};AUTHENTICATOR=snowflake_jwt;PRIVATE_KEY={_privateKey}"
        };

        await conn.OpenAsync();
        return conn;
    }

    // 🔹 Execute Query & Return Results (Async)
    public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql)
    {
        var results = new List<Dictionary<string, object>>();

        await using var conn = await GetConnectionAsync();
        await using var cmd = conn.CreateCommand();

        // Use correct syntax for Snowflake database switch
        cmd.CommandText = "USE OUTREACHGENIUS_DRIPS;";
        await cmd.ExecuteNonQueryAsync();

        // Execute actual query
        cmd.CommandText = sql;
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = await reader.GetFieldValueAsync<object>(i);
            }
            results.Add(row);
        }

        return results;
    }
}
