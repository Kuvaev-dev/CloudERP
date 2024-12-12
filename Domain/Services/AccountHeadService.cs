using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAccountHeadService
    {
        Task<IEnumerable<AccountHead>> GetAllAsync();
        Task<AccountHead> GetByIdAsync(int id);
        Task CreateAsync(AccountHead accountHead);
        Task UpdateAsync(AccountHead accountHead);
        Task DeleteAsync(int id);
    }

    public class AccountHeadService : IAccountHeadService
    {
        private readonly IAccountHeadRepository _repository;

        public AccountHeadService(IAccountHeadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AccountHead>> GetAllAsync()
        {
            var dbModels = await _repository.GetAllAsync();
            var accountHeads = dbModels.Select(AccountHeadMapper.MapToDomain).ToList();

            return accountHeads;
        }

        public async Task<AccountHead> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : AccountHeadMapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(AccountHead accountHead)
        {
            var dbModel = AccountHeadMapper.MapToDatabase(accountHead);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(AccountHead accountHead)
        {
            var dbModel = AccountHeadMapper.MapToDatabase(accountHead);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            if (dbModel == null) throw new KeyNotFoundException("AccountHead not found.");

            await _repository.DeleteAsync(dbModel);
        }
    }
}
