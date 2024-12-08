using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DatabaseAccess.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<tblCategory> GetAll(int companyID, int branchID);
        tblCategory GetById(int id);
        void Add(tblCategory category);
        void Update(tblCategory category);
        void Delete(tblCategory category);
    }

    public class CategoryRepository : ICategoryRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CategoryRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<tblCategory> GetAll(int companyID, int branchID)
        {
            return _dbContext.tblCategory
                .Include(c => c.tblUser)
                .Where(c => c.CompanyID == companyID && c.BranchID == branchID)
                .ToList();
        }

        public tblCategory GetById(int id)
        {
            return _dbContext.tblCategory.Find(id);
        }

        public void Add(tblCategory category)
        {
            _dbContext.tblCategory.Add(category);
            _dbContext.SaveChanges();
        }

        public void Update(tblCategory category)
        {
            var existingCategory = _dbContext.tblCategory.Find(category.CategoryID);
            if (existingCategory == null) throw new KeyNotFoundException("Category not found.");

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.UserID = category.UserID;

            _dbContext.Entry(existingCategory).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void Delete(tblCategory category)
        {
            var existingCategory = _dbContext.tblCategory.Find(category.CategoryID);
            if (existingCategory == null) throw new KeyNotFoundException("Category not found.");

            _dbContext.tblCategory.Remove(existingCategory);
            _dbContext.SaveChanges();
        }
    }
}
