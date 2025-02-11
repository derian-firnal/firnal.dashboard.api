using firnal.dashboard.data;

namespace firnal.dashboard.services.Interfaces
{
    public interface ICampaignService
    {
        Task<int> GetTodaysUsersCountAsync();
        Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync();
        Task<List<Heatmap>> GetDistinctZips();
        Task<List<Campaign>> GetAll();
    }
}
