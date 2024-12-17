using DatabaseAccess;
using DatabaseAccess.Repositories;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAccountActivityService
    {
        Task<IEnumerable<AccountActivity>> GetAllAsync();
        Task<AccountActivity> GetByIdAsync(int id);
    }

    public class AccountActivityService : IAccountActivityService
    {
        private readonly IAccountActivityRepository _repository;
        private readonly IMapper<AccountActivity, tblAccountActivity> _mapper;

        public AccountActivityService(IAccountActivityRepository repository, IMapper<AccountActivity, tblAccountActivity> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountActivity>> GetAllAsync()
        {
            var dbModels = await _repository.GetAllAsync();
            var accountActivities = dbModels.Select(_mapper.MapToDomain).ToList();

            return accountActivities;
        }

        public async Task<AccountActivity> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }
    }
}
