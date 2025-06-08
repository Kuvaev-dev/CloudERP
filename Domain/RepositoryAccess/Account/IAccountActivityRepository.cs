using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IAccountActivityRepository
    {
        Task<IEnumerable<AccountActivity>> GetAllAsync();
        Task<AccountActivity> GetByIdAsync(int id);
        Task AddAsync(AccountActivity accountActivity);
        Task UpdateAsync(AccountActivity accountActivity);
        Task<bool> IsExists(AccountActivity accountActivity);
    }
}
