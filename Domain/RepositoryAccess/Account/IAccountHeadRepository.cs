using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IAccountHeadRepository
    {
        Task<IEnumerable<AccountHead>> GetAllAsync();
        Task<IEnumerable<int>> GetAllIdsAsync();
        Task<AccountHead> GetByIdAsync(int id);
        Task AddAsync(AccountHead accountHead);
        Task UpdateAsync(AccountHead accountHead);
        Task<bool> IsExists(AccountHead accountHead);
    }
}
