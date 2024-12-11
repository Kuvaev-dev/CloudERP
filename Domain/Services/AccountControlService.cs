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
        AccountControl GetById(int id);
        void Create(AccountControl accountControl);
        void Update(AccountControl accountControl);
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
            var dbModels = _repository.GetAll(companyId, branchId);
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

        public AccountControl GetById(int id)
        {
            var dbModel = _repository.GetById(id);
            return dbModel == null ? null : AccountControlMapper.MapToDomain(dbModel);
        }

        public void Create(AccountControl accountControl)
        {
            var dbModel = AccountControlMapper.MapToDatabase(accountControl);
            _repository.Add(dbModel);
        }

        public void Update(AccountControl accountControl)
        {
            var dbModel = _repository.GetById(accountControl.AccountControlID);
            if (dbModel == null) throw new KeyNotFoundException("AccountControl not found.");

            var updatedModel = AccountControlMapper.MapToDatabase(accountControl);
            _repository.Update(updatedModel);
        }
    }
}
