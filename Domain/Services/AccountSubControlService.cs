using DatabaseAccess;
using DatabaseAccess.Repositories;
using Domain.Mapping.Base;
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
        private readonly IMapper<AccountSubControl, tblAccountSubControl> _mapper;

        public AccountSubControlService(IAccountSubControlRepository repository, IMapper<AccountSubControl, tblAccountSubControl> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountSubControl>> GetAllAsync(int companyId, int branchId)
        {
            var accountSubControls = await _repository.GetAllAsync(companyId, branchId);
            return accountSubControls.Select(_mapper.MapToDomain);
        }

        public async Task<AccountSubControl> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.MapToDomain(entity);
        }

        public async Task CreateAsync(AccountSubControl accountSubControl)
        {
            var dbModel = _mapper.MapToDatabase(accountSubControl);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(AccountSubControl accountSubControl)
        {
            var dbModel = _mapper.MapToDatabase(accountSubControl);
            await _repository.UpdateAsync(dbModel);
        }
    }
}
