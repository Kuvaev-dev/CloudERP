using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;

namespace Domain.Services
{
    public interface IAccountControlService
    {
        Task<IEnumerable<AccountControl>> GetAllAsync(int companyId, int branchId);
        Task<AccountControl> GetByIdAsync(int id);
        Task CreateAsync(AccountControl accountControl);
        Task UpdateAsync(AccountControl accountControl);
    }

    public class AccountControlService : IAccountControlService
    {
        private readonly IAccountControlRepository _repository;
        private readonly IAccountHeadRepository _headRepository;

        public AccountControlService(IAccountControlRepository repository, IAccountHeadRepository headRepository)
        {
            _repository = repository;
            _headRepository = headRepository;
        }

        public async Task<IEnumerable<AccountControl>> GetAllAsync(int companyId, int branchId)
        {
            var dbModels = await _repository.GetAllAsync(companyId, branchId);
            var accountHeads = await _headRepository.GetAllAsync();
            var headsDictionary = accountHeads.ToDictionary(x => x.AccountHeadID, x => x.AccountHeadName);

            return dbModels.Select(dbModel =>
            {
                var accountControl = AccountControlMapper.MapToDomain(dbModel);
                accountControl.FullName = dbModel.tblUser?.FullName ?? "Unknown";
                accountControl.AccountHeadName = headsDictionary.ContainsKey(dbModel.AccountHeadID)
                                                 ? headsDictionary[dbModel.AccountHeadID]
                                                 : "Unknown";
                return accountControl;
            }).ToList();
        }

        public async Task<AccountControl> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : AccountControlMapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(AccountControl accountControl)
        {
            var dbModel = AccountControlMapper.MapToDatabase(accountControl);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(AccountControl accountControl)
        {
            var dbModel = await _repository.GetByIdAsync(accountControl.AccountControlID);
            if (dbModel == null) throw new KeyNotFoundException("AccountControl not found.");

            var updatedModel = AccountControlMapper.MapToDatabase(accountControl);
            await _repository.UpdateAsync(updatedModel);
        }
    }
}
