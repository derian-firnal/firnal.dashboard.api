using firnal.dashboard.data;

namespace firnal.dashboard.repositories.Interfaces
{
    public interface ICampaignRepository
    {
        Task<int> GetTodaysUsersCountAsync(string schemaName);
        Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync(string schemaName);
        Task<List<Heatmap>> GetDistinctZips(string schemaName);
        Task<List<Campaign>> GetAll(string schemaName);
    }
}
