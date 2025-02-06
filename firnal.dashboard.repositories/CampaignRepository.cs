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

        public Task<int> AddAsync(Campaign entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Campaign>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Campaign> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetTodaysUsersCountAsync()
        {
            using var conn = _dbFactory.GetConnection();

            return await conn.ExecuteScalarAsync<int>("SELECT count(distinct first_name, last_name) FROM OUTREACHGENIUS_DRIPS.SHEET1.campaign");
        }

        public Task<int> UpdateAsync(Campaign entity)
        {
            throw new NotImplementedException();
        }
    }
}
