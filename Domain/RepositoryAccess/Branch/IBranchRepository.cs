using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Branch>> GetByCompanyAsync(int companyId);
        Task<IEnumerable<Branch>> GetSubBranchAsync(int companyId, int branchId);
        Task<List<int>> GetBranchIDsAsync(int branchID);
        Task<Branch> GetByIdAsync(int id);
        Task AddAsync(Branch branch);
        Task UpdateAsync(Branch branch);
    }
}
