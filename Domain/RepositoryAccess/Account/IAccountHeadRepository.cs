using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IAccountHeadRepository
    {
        Task<IEnumerable<AccountHead>> GetAllAsync();
        Task<AccountHead> GetByIdAsync(int id);
        Task AddAsync(AccountHead accountHead);
        Task UpdateAsync(AccountHead accountHead);
    }
}
