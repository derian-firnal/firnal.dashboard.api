using Dapper;
using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace firnal.dashboard.repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly SnowflakeDbConnectionFactory _dbFactory;
        private readonly IMemoryCache _cache;
        private const string DbName = "OUTREACHGENIUS_DRIPS";
        private const string Schema = "fides";

        public CampaignRepository(SnowflakeDbConnectionFactory dbFactory, IMemoryCache cache)
        {
            _dbFactory = dbFactory;
            _cache = cache;
        }

        private MemoryCacheEntryOptions GetCacheOptionsForMidnight()
        {
            // Set cache expiration at the next midnight UTC.
            var midnightUtc = DateTimeOffset.UtcNow.Date.AddDays(1);
            return new MemoryCacheEntryOptions { AbsoluteExpiration = midnightUtc };
        }

        public async Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync(string schemaName)
        {
            string cacheKey = $"CampaignUserDetails_{schemaName}";
            //if (_cache.TryGetValue(cacheKey, out List<CampaignUserDetails>? cachedDetails) && cachedDetails != null)
            //{
            //    return cachedDetails;
            //}

            using var conn = _dbFactory.GetConnection();
            var sql = $"SELECT first_name, last_name, personal_phone, gender, age_range, income_range, net_worth FROM {DbName}.{schemaName}.campaign";
            var result = await conn.QueryAsync<CampaignUserDetails>(sql);
            var details = result.ToList();

            _cache.Set(cacheKey, details, GetCacheOptionsForMidnight());
            return details;
        }

        public async Task<List<Heatmap>> GetDistinctZips(string schemaName)
        {
            string cacheKey = $"DistinctZips_{schemaName}";
            //if (_cache.TryGetValue(cacheKey, out List<Heatmap>? cachedZips) && cachedZips != null)
            //{
            //    return cachedZips;
            //}

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
            var heatmaps = result.ToList();

            _cache.Set(cacheKey, heatmaps, GetCacheOptionsForMidnight());
            return heatmaps;
        }

        public async Task<int> GetTotalUsersAsync(string schemaName)
        {
            string cacheKey = $"TotalUsers_{schemaName}";
            //if (_cache.TryGetValue(cacheKey, out int cachedTotal))
            //{
            //    return cachedTotal;
            //}

            using var conn = _dbFactory.GetConnection();
            int totalUsers = await conn.ExecuteScalarAsync<int>($"SELECT count(distinct first_name, last_name) FROM {DbName}.{schemaName}.campaign");
            _cache.Set(cacheKey, totalUsers, GetCacheOptionsForMidnight());
            return totalUsers;
        }

        public async Task<List<Campaign>> GetAll(string schemaName)
        {
            string cacheKey = $"AllCampaigns_{schemaName}";
            //if (_cache.TryGetValue(cacheKey, out List<Campaign>? cachedCampaigns) && cachedCampaigns != null)
            //{
            //    return cachedCampaigns;
            //}

            using var conn = _dbFactory.GetConnection();
            var result = await conn.QueryAsync<Campaign>($"SELECT * FROM {DbName}.{schemaName}.campaign");
            var campaigns = result.ToList();

            _cache.Set(cacheKey, campaigns, GetCacheOptionsForMidnight());
            return campaigns;
        }

        public async Task<int> GetNewUsersAsync(string schemaName)
        {
            string cacheKey = $"NewUsers_{schemaName}";
            //if (_cache.TryGetValue(cacheKey, out int cachedNewUsers))
            //{
            //    return cachedNewUsers;
            //}

            try
            {
                using var conn = _dbFactory.GetConnection();
                var sql = $@"
                SELECT COUNT(distinct first_name, last_name) 
                FROM {DbName}.{schemaName}.campaign 
                WHERE TO_DATE(SUBSTR(""timestamp_incoming_webhook"", 1, 10), 'DD/MM/YYYY') = CURRENT_DATE;";
                int newUsers = await conn.ExecuteScalarAsync<int>(sql);
                _cache.Set(cacheKey, newUsers, GetCacheOptionsForMidnight());
                return newUsers;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<UsageData>> GetNewUsersOverPast7Days(string schemaName)
        {
            string cacheKey = $"NewUsersOverPast7Days_{schemaName}";
            //if (_cache.TryGetValue(cacheKey, out List<UsageData>? cachedUsersOver7Days))
            //{
            //    return cachedUsersOver7Days ?? new List<UsageData>();
            //}

            try
            {
                // Define the SQL query with the provided schema name.
                var query = $@"
                            SELECT 
                                TO_DATE(SUBSTR(""timestamp_incoming_webhook"", 1, 10), 'DD/MM/YYYY') AS UsageDate,
                                COUNT(DISTINCT first_name, last_name) AS UsageCount
                            FROM {DbName}.{schemaName}.campaign
                            WHERE TO_DATE(SUBSTR(""timestamp_incoming_webhook"", 1, 10), 'DD/MM/YYYY')
                                  BETWEEN CURRENT_DATE - 7 AND CURRENT_DATE
                            GROUP BY TO_DATE(SUBSTR(""timestamp_incoming_webhook"", 1, 10), 'DD/MM/YYYY')
                            ORDER BY UsageDate;
                        ";

                using var conn = _dbFactory.GetConnection();
                var results = await conn.QueryAsync<UsageData>(query);
                _cache.Set(cacheKey , results, GetCacheOptionsForMidnight());

                return results.ToList();
            }
            catch
            {
                return new List<UsageData>();
            }
        }
    }
}
