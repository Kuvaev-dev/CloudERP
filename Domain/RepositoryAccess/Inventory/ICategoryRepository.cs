using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(int companyID, int branchID);
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task<bool> IsExists(Category category);
    }
}
