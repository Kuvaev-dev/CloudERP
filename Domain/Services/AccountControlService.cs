using System.Collections.Generic;
using System.Linq;
using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;

namespace Domain.Services
{
    public interface IAccountControlService
    {
        IEnumerable<AccountControl> GetAll(int companyId, int branchId);
        AccountControl GetById(int id);
        void Create(AccountControl accountControl);
        void Update(AccountControl accountControl);
    }

    public class AccountControlService : IAccountControlService
    {
        private readonly IAccountControlRepository _repository;

        public AccountControlService(IAccountControlRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<AccountControl> GetAll(int companyId, int branchId)
        {
            var dbModels = _repository.GetAll(companyId, branchId);
            return dbModels.Select(AccountControlMapper.MapToDomain).ToList();
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
