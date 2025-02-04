using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IAccountActivityRepository
    {
        Task<IEnumerable<AccountActivity>> GetAllAsync();
        Task<AccountActivity> GetByIdAsync(int id);
        Task AddAsync(AccountActivity accountActivity);
        Task UpdateAsync(AccountActivity accountActivity);
    }
}
