using firnal.dashboard.data;

namespace firnal.dashboard.services.Interfaces
{
    public interface ICampaignService
    {
        Task<int> GetTodaysUsersCountAsync();
        Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync();
    }
}
