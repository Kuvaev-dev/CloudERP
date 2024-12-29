using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CategoryRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int companyID, int branchID)
        {
            var entities = await _dbContext.tblCategory
            .Include(c => c.tblUser)
            .Where(c => c.CompanyID == companyID && c.BranchID == branchID)
            .ToListAsync();

            return entities.Select(c => new Category
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                BranchID = c.BranchID,
                BranchName = c.tblBranch.BranchName,
                CompanyID = c.CompanyID,
                CompanyName = c.tblCompany.Name,
                UserID = c.UserID,
                UserName = c.tblUser.UserName
            });
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblCategory.FindAsync(id);

            return entity == null ? null : new Category
            {
                CategoryID = entity.CategoryID,
                CategoryName = entity.CategoryName,
                BranchID = entity.BranchID,
                BranchName = entity.tblBranch.BranchName,
                CompanyID = entity.CompanyID,
                CompanyName = entity.tblCompany.Name,
                UserID = entity.UserID,
                UserName = entity.tblUser.UserName
            };
        }

        public async Task AddAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            var entity = new tblCategory
            {
                CategoryID = category.CategoryID,
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
            if (category == null) throw new ArgumentNullException(nameof(category));

            var entity = await _dbContext.tblCategory.FindAsync(category.CategoryID);
            if (entity == null) throw new KeyNotFoundException("Category not found.");

            entity.CategoryID = category.CategoryID;
            entity.CategoryName = category.CategoryName;
            entity.CompanyID = category.CompanyID;
            entity.BranchID = category.BranchID;
            entity.UserID = category.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
