﻿using firnal.dashboard.data;

namespace firnal.dashboard.repositories.Interfaces
{
    public interface ICampaignRepository
    {
        Task<int> GetTotalUsersAsync(string schemaName);
        Task<List<CampaignUserDetails>> GetCampaignUserDetailsAsync(string schemaName);
        Task<List<Heatmap>> GetDistinctZips(string schemaName);
        Task<List<Campaign>> GetAll(string schemaName);
        Task<int> GetNewUsersAsync(string schemaName);
        Task<List<UsageData>> GetNewUsersOverPast7Days(string schemaName);
    }
}
