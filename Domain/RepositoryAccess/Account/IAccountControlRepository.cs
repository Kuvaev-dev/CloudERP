using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IAccountControlRepository
    {
        Task<IEnumerable<AccountControl>> GetAllAsync(int companyId, int branchId);
        Task<AccountControl> GetByIdAsync(int id);
        Task AddAsync(AccountControl accountControl);
        Task UpdateAsync(AccountControl accountControl);
        Task<bool> IsExists(AccountControl accountControl);
    }
}
