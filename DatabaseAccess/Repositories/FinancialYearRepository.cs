using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IFinancialYearRepository
    {
        Task<IEnumerable<tblFinancialYear>> GetAllAsync();
        Task<IEnumerable<tblFinancialYear>> GetAllActiveAsync();
        Task<tblFinancialYear> GetSingleActiveAsync();
        Task<tblFinancialYear> GetByIdAsync(int id);
        Task AddAsync(tblFinancialYear financialYear);
        Task UpdateAsync(tblFinancialYear financialYear);
        Task DeleteAsync(int id);
    }

    public class FinancialYearRepository : IFinancialYearRepository
    {
        private readonly CloudDBEntities _dbContext;

        public FinancialYearRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblFinancialYear>> GetAllAsync()
        {
            return await _dbContext.tblFinancialYear.Include(u => u.tblUser).ToListAsync();
        }

        public async Task<IEnumerable<tblFinancialYear>> GetAllActiveAsync()
        {
            return await _dbContext.tblFinancialYear.Where(f => f.IsActive).ToListAsync();
        }

        public async Task<tblFinancialYear> GetSingleActiveAsync()
        {
            return await _dbContext.tblFinancialYear.FirstOrDefaultAsync(f => f.IsActive);
        }

        public async Task<tblFinancialYear> GetByIdAsync(int id)
        {
            return await _dbContext.tblFinancialYear.FindAsync(id);
        }

        public async Task AddAsync(tblFinancialYear financialYear)
        {
            _dbContext.tblFinancialYear.Add(financialYear);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblFinancialYear financialYear)
        {
            var existing = await _dbContext.tblFinancialYear.FindAsync(financialYear.FinancialYearID);
            if (existing == null) throw new KeyNotFoundException("Financial Year not found.");

            _dbContext.Entry(existing).CurrentValues.SetValues(financialYear);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var financialYear = await _dbContext.tblFinancialYear.FindAsync(id);
            if (financialYear == null) throw new KeyNotFoundException("Financial Year not found.");

            _dbContext.tblFinancialYear.Remove(financialYear);
            await _dbContext.SaveChangesAsync();
        }
    }
}
