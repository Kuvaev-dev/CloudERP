using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetAllAsync(int companyID, int branchID);
        Task<Stock> GetByIdAsync(int id);
        Task<Stock> GetByProductNameAsync(int companyID, int branchID, string productName);
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
        Task<int> GetTotalStockItemsByCompanyAsync(int companyID);
        Task<int> GetTotalAvaliableItemsByCompanyAsync(int companyID);
        Task<int> GetTotalExpiredItemsByCompanyAsync(int companyID);
        Task<bool> IsExists(Stock stock);
    }
}
