using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAccountSubControlService
    {
        Task<IEnumerable<AccountSubControl>> GetAllAsync(int companyId, int branchId);
        Task<AccountSubControl> GetByIdAsync(int id);
        Task CreateAsync(AccountSubControl accountSubControl);
        Task UpdateAsync(AccountSubControl accountSubControl);
    }

    public class AccountSubControlService : IAccountSubControlService
    {
        private readonly IAccountSubControlRepository _repository;

        public AccountSubControlService(IAccountSubControlRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AccountSubControl>> GetAllAsync(int companyId, int branchId)
        {
            var accountSubControls = await _repository.GetAllAsync(companyId, branchId);
            return accountSubControls.Select(AccountSubControlMapper.MapToDomain);
        }

        public async Task<AccountSubControl> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : AccountSubControlMapper.MapToDomain(entity);
        }

        public async Task CreateAsync(AccountSubControl accountSubControl)
        {
            var dbModel = AccountSubControlMapper.MapToDatabase(accountSubControl);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(AccountSubControl accountSubControl)
        {
            var dbModel = AccountSubControlMapper.MapToDatabase(accountSubControl);
            await _repository.UpdateAsync(dbModel);
        }
    }
}
