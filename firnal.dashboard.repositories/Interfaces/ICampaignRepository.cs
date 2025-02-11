using firnal.dashboard.data;

namespace firnal.dashboard.repositories.Interfaces
{
    public interface ICampaignRepository
    {
        Task<int> GetTodaysUsersCountAsync();
        Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync();
        Task<List<Heatmap>> GetDistinctZips();
        Task<List<Campaign>> GetAll();
    }
}
