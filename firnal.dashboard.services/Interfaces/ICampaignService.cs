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
        Task<GenderVariance> GetGenderVariance(string schemaName);
        Task<int> GetAverageIncome(string schemaName);
        Task<List<AgeRange>> GetAgeRange(string schemaName);
        Task<List<TopicData>> GetTopicBreakdown(string schemaName);
        Task<List<ProfessionData>> GetProfessionBreakdown(string schemaName);
    }
}
