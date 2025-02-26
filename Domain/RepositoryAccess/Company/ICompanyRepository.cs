using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company> GetByIdAsync(int id);
        Task<Company> GetByNameAsync(string name);
        Task<bool> CheckCompanyExistsAsync(string name);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
    }
}
