using Dapper;
using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;

namespace firnal.dashboard.repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly SnowflakeDbConnectionFactory _dbFactory;
        private const string DbName = "OUTREACHGENIUS_DRIPS";
        private const string Schema = "fides";

        public CampaignRepository(SnowflakeDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync(string schemaName)
        {
            using var conn = _dbFactory.GetConnection();

            var sql = $"SELECT first_name, last_name, personal_phone, gender, age_range, income_range, net_worth FROM {DbName}.{schemaName}.campaign";
            var result = await conn.QueryAsync<CampaignUserDetails>(sql);

            return result.ToList();
        }

        public async Task<List<Heatmap>> GetDistinctZips(string schemaName)
        {
            using var conn = _dbFactory.GetConnection();

            var sql = @$"SELECT 
                            TRY_CAST(c.personal_zip AS INTEGER) AS personal_zip, 
                            z.latitude, 
                            z.longitude, 
                            count(*) as zip_count 
                        FROM {DbName}.{schemaName}.campaign c
                        INNER JOIN OUTREACHGENIUS_DRIPS.public.zipcodes z 
                            ON TRY_CAST(c.personal_zip AS INTEGER) = z.postal_code
                        WHERE c.personal_zip IS NOT NULL AND c.personal_zip != ''
                        GROUP BY TRY_CAST(c.personal_zip as integer), z.latitude, z.longitude
                        ORDER BY zip_count DESC;";

            var result = await conn.QueryAsync<Heatmap>(sql);

            return result.ToList();
        }

        public async Task<int> GetTodaysUsersCountAsync(string schemaName)
        {
            using var conn = _dbFactory.GetConnection();

            return await conn.ExecuteScalarAsync<int>($"SELECT count(distinct first_name, last_name) FROM {DbName}.{schemaName}.campaign");
        }

        public async Task<List<Campaign>> GetAll(string schemaName)
        {
            using var conn = _dbFactory.GetConnection();

            var result = await conn.QueryAsync<Campaign>($"SELECT * FROM {DbName}.{schemaName}.campaign");

            return result.ToList();
        }
    }
}
