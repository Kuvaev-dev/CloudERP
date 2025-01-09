using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetAllAsync(int companyID, int branchID);
        Task<Stock> GetByIdAsync(int id);
        Task<Stock> GetByProductNameAsync(int companyID, int branchID, string productName);
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
    }
}
