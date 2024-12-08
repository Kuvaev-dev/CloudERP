using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    public interface IAccountSubControlService
    {
        IEnumerable<AccountSubControl> GetAll(int companyId, int branchId);
        AccountSubControl GetById(int id);
        void Create(AccountSubControl accountSubControl);
        void Update(AccountSubControl accountSubControl);
    }

    public class AccountSubControlService : IAccountSubControlService
    {
        private readonly IAccountSubControlRepository _repository;

        public AccountSubControlService(IAccountSubControlRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<AccountSubControl> GetAll(int companyId, int branchId)
        {
            return _repository.GetAll(companyId, branchId)
                              .Select(AccountSubControlMapper.MapToDomain);
        }

        public AccountSubControl GetById(int id)
        {
            var entity = _repository.GetById(id);
            return entity == null ? null : AccountSubControlMapper.MapToDomain(entity);
        }

        public void Create(AccountSubControl accountSubControl)
        {
            var dbModel = AccountSubControlMapper.MapToDatabase(accountSubControl);
            _repository.Add(dbModel);
        }

        public void Update(AccountSubControl accountSubControl)
        {
            var dbModel = AccountSubControlMapper.MapToDatabase(accountSubControl);
            _repository.Update(dbModel);
        }
    }
}
