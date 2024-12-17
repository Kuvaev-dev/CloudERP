using DatabaseAccess.Repositories;
using Domain.Mapping;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<IEnumerable<Supplier>> GetByCompanyAndBranchAsync(int companyID, int branchID);
        Task<Supplier> GetByIdAsync(int id);
        Task<Supplier> GetByNameAndContactAsync(int companyID, int branchID, string supplierName, string contactNo);
        Task<IEnumerable<Supplier>> GetSuppliersByBranchesAsync(int branchID);
        Task CreateAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);
    }

    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;
        private readonly SupplierMapper _mapper;

        public SupplierService(ISupplierRepository repository, SupplierMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var dbModels = await _repository.GetAllAsync();
            return dbModels.Select(_mapper.MapToDomain);
        }

        public async Task<IEnumerable<Supplier>> GetByCompanyAndBranchAsync(int companyID, int branchID)
        {
            var dbModels = await _repository.GetByCompanyAndBranchAsync(companyID, branchID);
            return dbModels.Select(_mapper.MapToDomain);
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            var dbModel = await _repository.GetByIdAsync(id);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task<Supplier> GetByNameAndContactAsync(int companyID, int branchID, string supplierName, string contactNo)
        {
            var dbModel = await _repository.GetByNameAndContactAsync(companyID, branchID, supplierName, contactNo);
            return dbModel == null ? null : _mapper.MapToDomain(dbModel);
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersByBranchesAsync(int branchID)
        {
            var dbModels = await _repository.GetSuppliersByBranchesAsync(branchID, _repository.DbContext);
            return dbModels.Select(_mapper.MapToDomain);
        }

        public async Task CreateAsync(Supplier supplier)
        {
            var dbModel = _mapper.MapToDatabase(supplier);
            await _repository.AddAsync(dbModel);
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            var dbModel = _mapper.MapToDatabase(supplier);
            await _repository.UpdateAsync(dbModel);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
