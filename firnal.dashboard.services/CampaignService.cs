using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;
using firnal.dashboard.services.Interfaces;
using System.Text;

namespace firnal.dashboard.services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;

        public CampaignService(ICampaignRepository campaignRepository)
        {
            _campaignRepository = campaignRepository;
        }

        public async Task<List<AgeRange>> GetAgeRange(string schemaName)
        {
            return await _campaignRepository.GetAgeRange(schemaName);
        }

        public async Task<byte[]> GetAll(string schemaName)
        {
            var result = await _campaignRepository.GetAll(schemaName);

            // Convert list to CSV format
            var csv = new StringBuilder();

            // Get headers dynamically from the Campaign class properties
            var properties = typeof(Campaign).GetProperties();
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Append data rows
            foreach (var campaign in result)
            {
                var values = properties.Select(p =>
                    p.GetValue(campaign, null)?.ToString()?.Replace(",", " ") ?? ""
                );
                csv.AppendLine(string.Join(",", values));
            }

            // Convert CSV to byte array
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return bytes;
        }

        public async Task<int> GetAverageIncome(string schemaName)
        {
            return await _campaignRepository.GetAverageIncome(schemaName);
        }

        public async Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync(string schemaName)
        {
            return await _campaignRepository.GetCampaignUserDetailsAsync(schemaName);
        }

        public async Task<List<Heatmap>> GetDistinctZips(string schemaName)
        {
            return await _campaignRepository.GetDistinctZips(schemaName);
        }

        public async Task<GenderVariance> GetGenderVariance(string schemaName)
        {
            return await _campaignRepository.GetGenderVariance(schemaName);
        }

        public async Task<int> GetNewUsersAsync(string schemaName)
        {
            return await _campaignRepository.GetNewUsersAsync(schemaName);
        }

        public async Task<List<UsageData>> GetNewUsersOverPast7Days(string schemaName)
        {
            return await _campaignRepository.GetNewUsersOverPast7Days(schemaName);
        }

        public async Task<List<TopicData>> GetTopicBreakdown(string schemaName)
        {
            return await _campaignRepository.GetTopicBreakdown(schemaName);
        }

        public async Task<int> GetTotalUsersAsync(string schemaName)
        {
            return await _campaignRepository.GetTotalUsersAsync(schemaName);
        }
    }
}
