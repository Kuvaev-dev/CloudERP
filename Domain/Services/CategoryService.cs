using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync(int companyID, int branchID);
        Task<Category> GetByIdAsync(int id);
        Task CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int companyID, int branchID)
        {
            var categories = await _repository.GetAllAsync(companyID, branchID);
            return categories.Select(CategoryMapper.MapToDomain).ToList();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : CategoryMapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(Category category)
        {
            var dbModel = CategoryMapper.MapToDatabase(category);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Category category)
        {
            var dbModel = CategoryMapper.MapToDatabase(category);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            if (dbModel == null) throw new KeyNotFoundException("Category not found.");
            await _repository.DeleteAsync(dbModel);
        }
    }
}
