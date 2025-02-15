using firnal.dashboard.data;

namespace firnal.dashboard.services.Interfaces
{
    public interface ICampaignService
    {
        Task<int> GetTotalUsersAsync(string schemaName);
        Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync(string schemaName);
        Task<List<Heatmap>> GetDistinctZips(string schemaName);
        Task<byte[]> GetAll(string schemaName);
        Task<int> GetNewUsersAsync(string schemaName);
        Task<List<UsageData>> GetNewUsersOverPast7Days(string schemaName);
    }
}
