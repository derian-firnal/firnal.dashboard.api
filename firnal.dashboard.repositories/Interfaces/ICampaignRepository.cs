namespace firnal.dashboard.repositories.Interfaces
{
    public interface ICampaignRepository : IRepository<Campaign>
    {
        Task<int> GetTodaysUsersCountAsync();
    }
}
