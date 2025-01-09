using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IAccountActivityRepository
    {
        Task<IEnumerable<AccountActivity>> GetAllAsync();
        Task<AccountActivity> GetByIdAsync(int id);
    }
}
