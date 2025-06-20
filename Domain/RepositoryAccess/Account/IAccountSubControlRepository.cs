﻿using Domain.Models;

namespace Domain.RepositoryAccess
{
    public interface IAccountSubControlRepository
    {
        Task<IEnumerable<AccountSubControl>> GetAllAsync(int companyId, int branchId);
        Task<AccountSubControl> GetByIdAsync(int id);
        Task<AccountSubControl> GetBySettingAsync(int id, int companyId, int branchId);
        Task AddAsync(AccountSubControl entity);
        Task UpdateAsync(AccountSubControl entity);
        Task<bool> IsExists(AccountSubControl entity);
    }
}
