using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByBranchAsync(int companyId, int branchTypeId, int branchId);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPasswordCodesAsync(string id, DateTime expiration);
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> IsExists(User user);
    }
}
