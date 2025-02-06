namespace firnal.dashboard.services.Interfaces
{
    public interface ICampaignService
    {
        Task<IEnumerable<Campaign>> GetAllUsersAsync();
        Task<Campaign> GetUserByIdAsync(int id);
        Task<int> AddUserAsync(Campaign user);
        Task<int> UpdateUserAsync(Campaign user);
        Task<int> DeleteUserAsync(int id);
        Task<int> GetTodaysUsersCountAsync();
    }
}
