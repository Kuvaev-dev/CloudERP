using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IFinancialYearRepository
    {
        Task<IEnumerable<FinancialYear>> GetAllAsync();
        Task<IEnumerable<FinancialYear>> GetAllActiveAsync();
        Task<FinancialYear> GetSingleActiveAsync();
        Task<FinancialYear> GetByIdAsync(int id);
        Task AddAsync(FinancialYear financialYear);
        Task UpdateAsync(FinancialYear financialYear);
        Task<bool> IsExists(FinancialYear financialYear);
    }
}
