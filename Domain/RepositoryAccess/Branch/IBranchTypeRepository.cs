using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IBranchTypeRepository
    {
        Task<IEnumerable<BranchType>> GetAllAsync();
        Task<BranchType> GetByIdAsync(int id);
        Task AddAsync(BranchType branchType);
        Task UpdateAsync(BranchType branchType);
    }
}
