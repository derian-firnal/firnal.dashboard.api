using firnal.dashboard.data;

namespace firnal.dashboard.services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
    }
}
