using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Inventory
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CategoryRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int companyID, int branchID)
        {
            var entities = await _dbContext.tblCategory
                .Include(c => c.User)
                .Include(c => c.Company)
                .Include(c => c.Branch)
                .Where(c => c.CompanyID == companyID && c.BranchID == branchID)
                .ToListAsync();

            return entities.Select(c => new Category
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                BranchID = c.BranchID,
                BranchName = c.Branch.BranchName,
                CompanyID = c.CompanyID,
                CompanyName = c.Company.Name,
                UserID = c.UserID,
                UserName = c.User.UserName
            });
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblCategory
                .Include(c => c.Branch)
                .Include(c => c.Company)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            return entity == null ? null : new Category
            {
                CategoryID = entity.CategoryID,
                CategoryName = entity.CategoryName,
                BranchID = entity.BranchID,
                BranchName = entity.Branch.BranchName,
                CompanyID = entity.CompanyID,
                CompanyName = entity.Company.Name,
                UserID = entity.UserID,
                UserName = entity.User.UserName
            };
        }

        public async Task AddAsync(Category category)
        {
            var entity = new tblCategory
            {
                CategoryName = category.CategoryName,
                BranchID = category.BranchID,
                CompanyID = category.CompanyID,
                UserID = category.UserID
            };

            _dbContext.tblCategory.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var entity = await _dbContext.tblCategory.FindAsync(category.CategoryID);

            entity.CategoryID = category.CategoryID;
            entity.CategoryName = category.CategoryName;

            _dbContext.tblCategory.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
