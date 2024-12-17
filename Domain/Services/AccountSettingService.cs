using DatabaseAccess.Repositories;
using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAccountSettingService
    {
        Task<IEnumerable<AccountSetting>> GetAllAsync(int companyId, int branchId);
        Task<AccountSetting> GetByIdAsync(int id);
        Task CreateAsync(AccountSetting accountSetting);
        Task UpdateAsync(AccountSetting accountSetting);
    }

    public class AccountSettingService : IAccountSettingService
    {
        private readonly IAccountSettingRepository _repository;
        private readonly IMapper<AccountSetting, tblAccountSetting> _mapper;

        public AccountSettingService(
            IAccountSettingRepository repository,
            IMapper<AccountSetting, tblAccountSetting> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountSetting>> GetAllAsync(int companyId, int branchId)
        {
            var dbModels = await _repository.GetAllAsync(companyId, branchId);
            return dbModels.Select(_mapper.MapToDomain);
        }

        public async Task<AccountSetting> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(AccountSetting accountSetting)
        {
            var dbModel = _mapper.MapToDatabase(accountSetting);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(AccountSetting accountSetting)
        {
            var dbModel = _mapper.MapToDatabase(accountSetting);
            await _repository.UpdateAsync(dbModel);
        }
    }
}
