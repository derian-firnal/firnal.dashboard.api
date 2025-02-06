using firnal.dashboard.repositories.Interfaces;
using firnal.dashboard.services.Interfaces;

namespace firnal.dashboard.services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;

        public CampaignService(ICampaignRepository campaignRepository)
        {
            _campaignRepository = campaignRepository;
        }

        public async Task<int> AddUserAsync(Campaign user)
        {
            return await _campaignRepository.AddAsync(user);
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            return await _campaignRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Campaign>> GetAllUsersAsync()
        {
            return await _campaignRepository.GetAllAsync();
        }

        public async Task<int> GetTodaysUsersCountAsync()
        {
            return await _campaignRepository.GetTodaysUsersCountAsync();
        }

        public async Task<Campaign> GetUserByIdAsync(int id)
        {
            return await _campaignRepository.GetByIdAsync(id);
        }

        public async Task<int> UpdateUserAsync(Campaign user)
        {
            return await _campaignRepository.UpdateAsync(user);
        }
    }
}
