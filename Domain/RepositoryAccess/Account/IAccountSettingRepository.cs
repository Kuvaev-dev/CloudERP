using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface IAccountSettingRepository
    {
        Task<IEnumerable<AccountSetting>> GetAllAsync(int companyId, int branchId);
        Task<AccountSetting> GetByIdAsync(int id);
        Task<AccountSetting> GetByActivityAsync(int id, int companyId, int branchId);
        Task SetDefault(int companyId, int branchId);
        Task AddAsync(AccountSetting accountSetting);
        Task UpdateAsync(AccountSetting accountSetting);
    }
}
