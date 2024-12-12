using DatabaseAccess;
using DatabaseAccess.Repositories;
using Domain.Mapping.Base;
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
        private readonly IMapper<AccountHead, tblAccountHead> _mapper;

        public AccountHeadService(IAccountHeadRepository repository, IMapper<AccountHead, tblAccountHead> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountHead>> GetAllAsync()
        {
            var dbModels = await _repository.GetAllAsync();
            var accountHeads = dbModels.Select(_mapper.MapToDomain).ToList();

            return accountHeads;
        }

        public async Task<AccountHead> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(AccountHead accountHead)
        {
            var dbModel = _mapper.MapToDatabase(accountHead);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(AccountHead accountHead)
        {
            var dbModel = _mapper.MapToDatabase(accountHead);
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
