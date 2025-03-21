using Dapper;
using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace firnal.dashboard.repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly SnowflakeDbConnectionFactory _dbFactory;
        private readonly IMemoryCache _cache;
        private const string DbName = "OUTREACHGENIUS_DRIPS";

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
            var sql = $"SELECT first_name, last_name, mobile_phone as personal_phone, gender, age_range, income_range, net_worth FROM {DbName}.{schemaName}.campaign";
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
                            SELECT
                                COUNT(distinct first_name, last_name) 
                            FROM 
                                {DbName}.{schemaName}.campaign 
                            WHERE 
                                TO_DATE(SUBSTR(created_at, 1, 10), 'YYYY-MM-DD') = CURRENT_DATE;";
                
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
                var query = $@" SELECT 
                                    TO_DATE(created_at) AS UsageDate,  -- Handles both 'YYYY-MM-DD' and full timestamp format
                                    COUNT(DISTINCT first_name || last_name) AS UsageCount
                                FROM 
                                    {DbName}.{schemaName}.campaign
                                WHERE 
                                    TO_DATE(created_at) BETWEEN CURRENT_DATE - 7 AND CURRENT_DATE
                                GROUP BY 
                                    TO_DATE(created_at)
                                ORDER BY 
                                    UsageDate;";

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

        public async Task<GenderVariance> GetGenderVariance(string schemaName)
        {
            try
            {
                var sql = $@" SELECT 
                              CASE 
                                WHEN gender = 'M' THEN 'Male'
                                WHEN gender = 'F' THEN 'Female'
                                ELSE 'Other'
                              END AS gender_full,
                              ROUND((COUNT(*) * 100.0) / SUM(COUNT(*)) OVER (), 2) AS percentage
                            FROM 
                                {DbName}.{schemaName}.campaign
                            GROUP BY 
                              CASE 
                                WHEN gender = 'M' THEN 'Male'
                                WHEN gender = 'F' THEN 'Female'
                                ELSE 'Other'
                              END;";

                // You can use Query<dynamic> if you don't want to create a temporary class
                using var conn = _dbFactory.GetConnection();
                var results = (await conn.QueryAsync(sql)).ToList();

                // Create a new GenderVariance object
                var genderVariance = new GenderVariance();

                foreach (var row in results)
                    if (row.GENDER_FULL == "Male")
                        genderVariance.Male = Convert.ToInt32(row.PERCENTAGE);
                    else if (row.GENDER_FULL == "Female")
                        genderVariance.Female = Convert.ToInt32(row.PERCENTAGE);

                // Now genderVariance contains the percentage for Male and Female
                return genderVariance;
            }
            catch
            {
                return new GenderVariance();
            }
        }

        public async Task<int> GetAverageIncome(string schemaName)
        {
            try
            {
                var sql = $@" SELECT ROUND(
                                     AVG(
                                       CASE 
                                         WHEN INCOME_RANGE ILIKE '% to %' THEN 
                                           (
                                             TO_NUMBER(REPLACE(REPLACE(SPLIT_PART(INCOME_RANGE, ' to ', 1), '$', ''), ',', '')) +
                                             TO_NUMBER(REPLACE(REPLACE(SPLIT_PART(INCOME_RANGE, ' to ', 2), '$', ''), ',', ''))
                                           ) / 2
                                         WHEN INCOME_RANGE ILIKE '%+%' THEN 
                                           TO_NUMBER(REPLACE(REPLACE(REPLACE(INCOME_RANGE, '$', ''), '+', ''), ',', ''))
                                         ELSE NULL
                                       END
                                     ), 0) AS avg_income
                            FROM {DbName}.{schemaName}.CAMPAIGN;";

                using var conn = _dbFactory.GetConnection();
                var result = await conn.QuerySingleAsync<int>(sql);

                return result;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<AgeRange>> GetAgeRange(string schemaName)
        {
            try
            {
                var sql = $@"SELECT
                              age_range,
                              COUNT(*) AS count
                            FROM {DbName}.{schemaName}.CAMPAIGN
                            GROUP BY age_range
                            ORDER BY count DESC;
                            ";

                using var conn = _dbFactory.GetConnection();
                var result = await conn.QueryAsync<AgeRange>(sql);

                return result.ToList();
            }
            catch
            {
                return new List<AgeRange>();
            }
        }

        public async Task<List<TopicData>> GetTopicBreakdown(string schemaName)
        {
            try
            {
                var sql = $@"SELECT
                              topic,
                              COUNT(*) AS count
                            FROM {DbName}.{schemaName}.CAMPAIGN
                            GROUP BY topic
                            ORDER BY count DESC;";

                using var conn = _dbFactory.GetConnection();
                var result = await conn.QueryAsync<TopicData>(sql);

                return result.ToList();
            }
            catch
            {
                return new List<TopicData>();
            }
        }
    }
}
