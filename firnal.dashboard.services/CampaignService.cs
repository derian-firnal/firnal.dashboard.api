using firnal.dashboard.data;
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

        public async Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync()
        {
            return await _campaignRepository.GetCampaignUserDetailsAsync();
        }

        public async Task<List<Heatmap>> GetDistinctZips()
        {
            return await _campaignRepository.GetDistinctZips();
        }

        public async Task<int> GetTodaysUsersCountAsync()
        {
            return await _campaignRepository.GetTodaysUsersCountAsync();
        }
    }
}
