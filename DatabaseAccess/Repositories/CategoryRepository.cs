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
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving categories.", ex);
            }
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving category with ID {id}.", ex);
            }
        }

        public async Task AddAsync(Category category)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new category.", ex);
            }
        }

        public async Task UpdateAsync(Category category)
        {
            try
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
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating category with ID {category.CategoryID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
