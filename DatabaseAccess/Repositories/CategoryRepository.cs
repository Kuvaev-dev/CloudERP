using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<tblCategory>> GetAllAsync(int companyID, int branchID);
        Task<tblCategory> GetByIdAsync(int id);
        Task AddAsync(tblCategory category);
        Task UpdateAsync(tblCategory category);
        Task DeleteAsync(tblCategory category);
    }

    public class CategoryRepository : ICategoryRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CategoryRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblCategory>> GetAllAsync(int companyID, int branchID)
        {
            return await _dbContext.tblCategory
                .Include(c => c.tblUser)
                .Where(c => c.CompanyID == companyID && c.BranchID == branchID)
                .ToListAsync();
        }

        public async Task<tblCategory> GetByIdAsync(int id)
        {
            return await _dbContext.tblCategory.FindAsync(id);
        }

        public async Task AddAsync(tblCategory category)
        {
            _dbContext.tblCategory.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblCategory category)
        {
            var existingCategory = await _dbContext.tblCategory.FindAsync(category.CategoryID);
            if (existingCategory == null) throw new KeyNotFoundException("Category not found.");

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.UserID = category.UserID;

            _dbContext.Entry(existingCategory).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(tblCategory category)
        {
            var existingCategory = await _dbContext.tblCategory.FindAsync(category.CategoryID);
            if (existingCategory == null) throw new KeyNotFoundException("Category not found.");

            _dbContext.tblCategory.Remove(existingCategory);
            await _dbContext.SaveChangesAsync();
        }
    }
}
