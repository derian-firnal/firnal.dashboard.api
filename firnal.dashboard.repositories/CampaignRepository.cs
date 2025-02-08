using Dapper;
using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;

namespace firnal.dashboard.repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly SnowflakeDbConnectionFactory _dbFactory;

        public CampaignRepository(SnowflakeDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync()
        {
            using var conn = _dbFactory.GetConnection();

            var sql = "SELECT first_name, last_name, personal_phone, gender, age_range, income_range, net_worth FROM OUTREACHGENIUS_DRIPS.SHEET1.campaign";
            var result = await conn.QueryAsync<CampaignUserDetails>(sql);

            return result.ToList();
        }

        public async Task<int> GetTodaysUsersCountAsync()
        {
            using var conn = _dbFactory.GetConnection();

            return await conn.ExecuteScalarAsync<int>("SELECT count(distinct first_name, last_name) FROM OUTREACHGENIUS_DRIPS.SHEET1.campaign");
        }
    }
}
