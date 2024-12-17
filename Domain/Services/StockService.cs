using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IStockService
    {
        Task<IEnumerable<Stock>> GetAllAsync(int companyID, int branchID);
        Task<Stock> GetByIdAsync(int id);
        Task<Stock> GetByProductNameAsync(int companyID, int branchID, string productName);
        Task CreateAsync(Stock stock);
        Task UpdateAsync(Stock stock);
        Task DeleteAsync(int id);
    }

    public class StockService : IStockService
    {
        private readonly IStockRepository _repository;
        private readonly StockMapper _mapper;

        public StockService(IStockRepository repository, StockMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Stock>> GetAllAsync(int companyID, int branchID)
        {
            var dbModels = await _repository.GetAllAsync(companyID, branchID);
            return dbModels.Select(_mapper.MapToDomain);
        }

        public async Task<Stock> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task<Stock> GetByProductNameAsync(int companyID, int branchID, string productName)
        {
            var dbModel = await _repository.GetByProductNameAsync(companyID, branchID, productName);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task CreateAsync(Stock stock)
        {
            var dbModel = _mapper.MapToDatabase(stock);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Stock stock)
        {
            var dbModel = _mapper.MapToDatabase(stock);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
