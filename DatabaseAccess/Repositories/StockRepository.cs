using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IStockRepository
    {
        Task<IEnumerable<tblStock>> GetAllAsync(int companyID, int branchID);
        Task<tblStock> GetByIdAsync(int id);
        Task<tblStock> GetByProductNameAsync(int companyID, int branchID, string productName);
        Task AddAsync(tblStock stock);
        Task UpdateAsync(tblStock stock);
        Task DeleteAsync(int id);
    }

    public class StockRepository : IStockRepository
    {
        private readonly CloudDBEntities _dbContext;

        public StockRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblStock>> GetAllAsync(int companyID, int branchID)
        {
            return await _dbContext.tblStock
                .Where(s => s.CompanyID == companyID && s.BranchID == branchID)
                .ToListAsync();
        }

        public async Task<tblStock> GetByIdAsync(int id)
        {
            return await _dbContext.tblStock.FindAsync(id);
        }

        public async Task<tblStock> GetByProductNameAsync(int companyID, int branchID, string productName)
        {
            return await _dbContext.tblStock
                .Where(s => s.CompanyID == companyID && s.BranchID == branchID && s.ProductName == productName)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(tblStock stock)
        {
            _dbContext.tblStock.Add(stock);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblStock stock)
        {
            var existing = await _dbContext.tblStock.FindAsync(stock.ProductID);
            if (existing == null)
                throw new KeyNotFoundException("Stock not found.");

            _dbContext.Entry(existing).CurrentValues.SetValues(stock);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var stock = await _dbContext.tblStock.FindAsync(id);
            if (stock != null)
            {
                _dbContext.tblStock.Remove(stock);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
