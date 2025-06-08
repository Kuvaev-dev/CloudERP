using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IUserTypeRepository
    {
        Task<IEnumerable<UserType>> GetAllAsync();
        Task<UserType> GetByIdAsync(int id);
        Task AddAsync(UserType userType);
        Task UpdateAsync(UserType userType);
        Task<bool> IsExists(UserType userType);
    }
}
