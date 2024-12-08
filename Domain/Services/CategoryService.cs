using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAll(int companyID, int branchID);
        Category GetById(int id);
        void Create(Category category);
        void Update(Category category);
        void Delete(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Category> GetAll(int companyID, int branchID)
        {
            return _repository.GetAll(companyID, branchID)
                              .Select(CategoryMapper.MapToDomain)
                              .ToList();
        }

        public Category GetById(int id)
        {
            var dbModel = _repository.GetById(id);
            return dbModel == null ? null : CategoryMapper.MapToDomain(dbModel);
        }

        public void Create(Category category)
        {
            var dbModel = CategoryMapper.MapToDatabase(category);
            _repository.Add(dbModel);
        }

        public void Update(Category category)
        {
            var dbModel = CategoryMapper.MapToDatabase(category);
            _repository.Update(dbModel);
        }

        public void Delete(int id)
        {
            var dbModel = _repository.GetById(id);
            if (dbModel == null) throw new KeyNotFoundException("Category not found.");
            _repository.Delete(dbModel);
        }
    }
}
