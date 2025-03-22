using firnal.dashboard.data;
using firnal.dashboard.repositories.Interfaces;
using firnal.dashboard.services.Interfaces;

namespace firnal.dashboard.services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }
    }
}