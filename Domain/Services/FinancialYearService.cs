using DatabaseAccess;
using DatabaseAccess.Repositories;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IFinancialYearService
    {
        Task<IEnumerable<FinancialYear>> GetAllAsync();
        Task<IEnumerable<FinancialYear>> GetAllActiveAsync();
        Task<FinancialYear> GetSingleActiveAsync();
        Task<FinancialYear> GetByIdAsync(int id);
        Task CreateAsync(FinancialYear financialYear);
        Task UpdateAsync(FinancialYear financialYear);
    }

    public class FinancialYearService : IFinancialYearService
    {
        private readonly IFinancialYearRepository _repository;
        private readonly IMapper<FinancialYear, tblFinancialYear> _mapper;

        public FinancialYearService(IFinancialYearRepository repository, IMapper<FinancialYear, tblFinancialYear> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FinancialYear>> GetAllAsync()
        {
            var dbModels = await _repository.GetAllAsync();
            return dbModels.Select(_mapper.MapToDomain);
        }

        public async Task<FinancialYear> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(FinancialYear financialYear)
        {
            var dbModel = _mapper.MapToDatabase(financialYear);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(FinancialYear financialYear)
        {
            var dbModel = _mapper.MapToDatabase(financialYear);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task<IEnumerable<FinancialYear>> GetAllActiveAsync()
        {
            var dbModels = await _repository.GetAllActiveAsync();
            return dbModels.Select(_mapper.MapToDomain);
        }

        public async Task<FinancialYear> GetSingleActiveAsync()
        {
            var dbModel = await _repository.GetSingleActiveAsync();
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }
    }
}
