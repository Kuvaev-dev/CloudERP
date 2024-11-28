using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Services
{
    public interface IAccountHeadService
    {
        IEnumerable<AccountHead> GetAll();
        AccountHead GetById(int id);
        void Create(AccountHead accountHead);
        void Update(AccountHead accountHead);
        void Delete(int id);
    }

    public class AccountHeadService : IAccountHeadService
    {
        private readonly IAccountHeadRepository _repository;

        public AccountHeadService(IAccountHeadRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<AccountHead> GetAll()
        {
            var dbModels = _repository.GetAll();
            return dbModels.Select(AccountHeadMapper.MapToDomain).ToList();
        }

        public AccountHead GetById(int id)
        {
            var dbModel = _repository.GetById(id);
            return dbModel == null ? null : AccountHeadMapper.MapToDomain(dbModel);
        }

        public void Create(AccountHead accountHead)
        {
            var dbModel = AccountHeadMapper.MapToDatabase(accountHead);
            _repository.Add(dbModel);
        }

        public void Update(AccountHead accountHead)
        {
            var dbModel = AccountHeadMapper.MapToDatabase(accountHead);
            _repository.Update(dbModel);
        }

        public void Delete(int id)
        {
            var dbModel = _repository.GetById(id);
            if (dbModel == null) throw new KeyNotFoundException("AccountHead not found.");

            _repository.Delete(dbModel);
        }
    }
}
