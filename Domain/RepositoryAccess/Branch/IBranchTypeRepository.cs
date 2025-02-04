using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
