using Snowflake.Data.Client;
using System.Data;

public class SnowflakeService
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly string _privateKeyPath;

    public SnowflakeService(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("Snowflake") ?? throw new Exception("Snowflake connection string not found.");
        _privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "rsa_key.p8");
    }

    // 🔹 Generic method to get a connection (Key Pair)
    public IDbConnection GetConnection()
    {
        var conn = new SnowflakeDbConnection();

        if (!File.Exists(_privateKeyPath))
            throw new FileNotFoundException("Private key file not found.", _privateKeyPath);

        conn.ConnectionString = $"{_connectionString};AUTHENTICATOR=snowflake_jwt;PRIVATE_KEY_FILE={_privateKeyPath}";
        conn.Open();
        return conn;
    }

    // 🔹 Execute Query & Return Results
    public List<Dictionary<string, object>> ExecuteQuery(string sql)
    {
        using (var conn = GetConnection())
        using (var cmd = conn.CreateCommand())
        {

            cmd.CommandText = "USE DATABASE OUTREACHGENIUS_DRIPS;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = sql;
            var results = new List<Dictionary<string, object>>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    results.Add(row);
                }
            }

            return results;
        }
    }
}
